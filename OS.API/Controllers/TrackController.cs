﻿using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using OS.Data.Models;
using OS.Services.Repository;
using OS.Services.Jobs;
using OS.Services.Storage;
using Quartz;
using Microsoft.AspNetCore.Authorization;
using OS.API.Utilities;
using OS.Data.Dtos;
using OS.Services.Mappers;
namespace OS.API.Controllers;

[ApiController]
[Authorize]
[Route("api/tracks")]
public class TrackController(
    IRepository repository,
    ITempStorageService tempStorageService,
    IStorageService storageService,
    IScheduler scheduler, ILogger<TrackController> logger, IDtoMapper mapper) : Controller
{
    private readonly IRepository _repository = repository;
    private readonly ITempStorageService _tempStorageService = tempStorageService;
    private readonly IStorageService _storageService = storageService;
    private readonly IScheduler _scheduler = scheduler;
    private readonly IDtoMapper _mapper = mapper;
    private readonly ILogger<TrackController> _logger = logger;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TracksDto>), 200)]
    public async Task<IActionResult> GetTracks([FromQuery][Optional] Guid? albumId,
        [FromQuery][Optional] string? title, [FromQuery][Optional] Guid? artistId, [FromQuery][Optional] Guid? playlistId)
    {
        var userId = RequestUtilities.GetUserId(HttpContext);

        var tracks = await _repository.FindAllAsync<Track>(t =>
        (albumId == null || (t.Album != null && t.Album.Id == albumId)) &&
        (title == null || t.Name.Contains(title) &&
        (artistId == null || (t.Album != null && t.Album.Artist.Id == artistId)) &&
        (playlistId != null || t.Playlists.FirstOrDefault(x => x.Id == playlistId && x.User.Id == userId) != null)),
        [nameof(Album), nameof(Track.Artists), nameof(Track.StarredBy)]);


        var trackDtos = ToTrackDtoWithFavorites(tracks, userId);

        return Ok(trackDtos);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TracksDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetTrack([FromRoute] Guid id)
    {
        var track = await _repository.GetAsync<Track>(id, [nameof(Album), nameof(Track.Artists)]);
        if (track == null)
        {
            return NotFound();
        }

        return Ok(track);
    }

    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [HttpGet("{id}/stream")]
    public async Task<IActionResult> GetTrackStream([FromRoute] Guid id, [FromQuery] bool requestOrigin = false)
    {
        var track = await _repository.GetAsync<Track>(id, [nameof(Track.StoredEntities)]);
        var originFound = false;
        if (track == null)
        {
            return NotFound();
        }

        string mime = "audio/ogg";
         var streamToReturn = new MemoryStream();
        if (requestOrigin)
        {
            var origin = track.StoredEntities.FirstOrDefault(x => x.Type == StoredEntityType.Origin);
            if (origin == null)
            {
                _logger.LogWarning($"Couldn't find the origin of file for track {id}", id);
            }
            else
            {
                await using var originStream = await _storageService.GetFileStream((string)origin.Id.ToString());
                if (originStream == null)
                {
                    return Problem("Origin file was found in the database, however it couldn't be found in the storage");
                }
                await originStream.CopyToAsync(streamToReturn);
                mime = origin.Mime;
                originFound = true;
            }
        }
        if (!requestOrigin || !originFound)
        {
            var stream = track.StoredEntities.FirstOrDefault(x => x.Type == StoredEntityType.Stream);
            if (stream == null)
            {
                return Problem("Stream file was not found in the database");
            }
            await using var lossyStream = await _storageService.GetFileStream((string)stream.Id.ToString());
            if (lossyStream == null)
            {
                return Problem("Stream file was found in the database, however it couldn't be found in the storage");
            }
            await lossyStream.CopyToAsync(streamToReturn);

            if (streamToReturn.Length == 0)
            {
                return Problem("Stream file was found in the database, however it was empty");
            }
            mime = stream.Mime;
        }

        streamToReturn.Seek(0, SeekOrigin.Begin);
        HttpContext.Response.Headers.Append("X-Track-Duration", track.Duration.ToString());
            
        return File(streamToReturn, mime, true);
    }

    [HttpPost]
    [DisableRequestSizeLimit]
    [Authorize(Policy = "Contributor")]
    [ProducesResponseType(400)]
    [ProducesResponseType(200)]
    [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
    public async Task<IActionResult> Upload([FromForm] string description, [FromForm] DateTime clientDate, IFormFile? file)
    {
        if (file == null)
        {
            return BadRequest("No file provided");
        }
        var guid = Guid.NewGuid().ToString();
        var stream = file.OpenReadStream();
        try
        {
            var operationCompleted = await _tempStorageService.SaveFileAsync(stream, guid);
            if (!operationCompleted)
            {
                return BadRequest("Failed to save file, view logs for more information");
            }

            var jobData = new JobDataMap { { "file", guid } };
            await _scheduler.TriggerJob(ImportTracksJob.Key, jobData);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }

    }

    [HttpGet("star")]
    [ProducesResponseType(typeof(IEnumerable<TracksDto>), 200)]
    public async Task<IActionResult> GetStarredTracks()
    {
        var userId = RequestUtilities.GetUserId(HttpContext);
        var tracks = (await _repository.FindAllAsync<Track, TracksDto>(t => t.StarredBy.Any(x => x.Id == userId), [nameof(Album), nameof(Track.Artists)])).Select(x =>
        {
            x.IsStarred = true;
            return x;
        });
        return Ok(tracks);
    }

    [HttpPut("{id}/star")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> StarTrack([FromRoute] Guid id)
    {
        var userId = RequestUtilities.GetUserId(HttpContext);
        var user = await _repository.GetAsync<User>(userId);
        var track = await _repository.GetAsync<Track>(id, [nameof(Track.StarredBy)]);
        if (track == null)
        {
            return NotFound();
        }
        if (track.StarredBy.Any(x => x.Id == userId))
        {
            return Ok(ResponseUtilities.BuildWarning("The track is already starred by the user"));
        }
        track.StarredBy.Add(user!);
        await _repository.UpdateAsync(track);
        return Ok();
    }

    [HttpDelete("{id}/star")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UnstarTrack([FromRoute] Guid id)
    {
        var userId = RequestUtilities.GetUserId(HttpContext);
        var track = await _repository.GetAsync<Track>(id, [nameof(Track.StarredBy)]);
        if (track == null)
        {
            return NotFound();
        }
        var starredBy = track.StarredBy.FirstOrDefault(x => x.Id == userId);
        if (starredBy == null)
        {
            return Ok(ResponseUtilities.BuildWarning("The song wasn't starred by the user"));
        }

        track.StarredBy.Remove(starredBy);
        await _repository.UpdateAsync(track);
        return Ok();
    }

    [HttpPatch("{id}")]
    [Authorize(Policy = "Contributor")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> PatchTrack([FromRoute] Guid id, [FromBody] PayloadTrackDto trackDto)
    {
        var track = await _repository.GetAsync<Track>(id);
        if (track == null)
        {
            return NotFound();
        }
        if (trackDto.AlbumId != null)
        {
            var album = await _repository.GetAsync<Album>((Guid)trackDto.AlbumId);
            if (album == null)
            {
                return NotFound(ResponseUtilities.BuildError("Album not found"));
            }
            track.Album = album;
        }
        if (trackDto.Name != null)
        {
            track.Name = trackDto.Name;
        }
        if (trackDto.Duration != null)
        {
            track.Duration = (int)trackDto.Duration;
        }
        if (trackDto.ArtistIds != null)
        {
            var artists = await _repository.FindAllAsync<Artist>(x => trackDto.ArtistIds.Contains(x.Id));
            track.Artists = artists.ToList();
        }

        await _repository.UpdateAsync(track);
        return Ok();
    }

    private List<TracksDto> ToTrackDtoWithFavorites(IEnumerable<Track> tracks, Guid userId)
    {
        var tracksDto = new List<TracksDto>();
        foreach (var track in tracks)
        {
            var trackDto = _mapper.Map<TracksDto>(track);
            trackDto.IsStarred = track.StarredBy.Any(x => x.Id == userId);
            tracksDto.Add(trackDto);
        }
        return tracksDto.ToList();
    }
}