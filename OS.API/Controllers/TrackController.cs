using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using OS.Data.Models;
using OS.Services.Repository;
using OS.Services.Jobs;
using OS.Services.Storage;
using Quartz;
using Microsoft.AspNetCore.Authorization;
using OS.API.Utilities;

namespace OS.API.Controllers;

[ApiController]
[Authorize]
[Route("api/tracks")]
public class TrackController(
    IRepository repository,
    ITempStorageService tempStorageService,
    IStorageService storageService,
    IScheduler scheduler, ILogger<TrackController> logger) : Controller
{
    private readonly IRepository _repository = repository;
    private readonly ITempStorageService _tempStorageService = tempStorageService;
    private readonly IStorageService _storageService = storageService;
    private readonly IScheduler _scheduler = scheduler;
    private readonly ILogger<TrackController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> GetTracks([FromQuery][Optional] Guid albumId,
        [FromQuery][Optional] string? title)
    {
        var tracks = await _repository.FindAllAsync<Track>(t => (albumId == Guid.Empty || (t.Album != null && t.Album.Id == albumId)) && (title == null || t.Name.Contains(title)), [nameof(Album), nameof(Track.Artists)]);
        if (albumId != Guid.Empty)
        {
            tracks = tracks.Where(t => t.NavigationAlbumId == albumId);
        }

        if (title != null)
        {
            tracks = tracks.Where(t => t.Name.Contains(title));
        }

        return Ok(tracks);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTrack([FromRoute] Guid id)
    {
        var track = await _repository.GetAsync<Track>(id, [nameof(Album), nameof(Track.Artists)]);
        if (track == null)
        {
            return NotFound();
        }

        return Ok(track);
    }

    [HttpGet("{id}/stream")]
    public async Task<IActionResult> GetTrackStream([FromRoute] Guid id, [FromQuery] bool requestOrigin = false)
    {
        var track = await _repository.GetAsync<Track>(id, [nameof(Track.StoredEntities)]);
        if (track == null)
        {
            return NotFound();
        }

        var origin = track.StoredEntities.FirstOrDefault(x => x.Type == StoredEntityType.Origin);
        var stream = track.StoredEntities.FirstOrDefault(x => x.Type == StoredEntityType.Stream);
        if (requestOrigin)
        {
            if (origin == null)
            {
                _logger.LogWarning($"Couldn't find the origin of file for track {id}", id);
            }
            else
            {
                var originStream = await _storageService.GetFileStream((string)origin.Id.ToString());
                if (originStream == null)
                {
                    return Problem("Origin file was found in the database, however it couldn't be found in the storage");
                }
                return File(originStream, origin.Mime);
            }
        }
        if (stream == null)
        {
            return Problem("Stream file was not found in the database");
        }
        var streamStream = await _storageService.GetFileStream((string)stream.Id.ToString());
        if (streamStream == null)
        {
            return Problem("Stream file was found in the database, however it couldn't be found in the storage");
        }
        return File(streamStream, stream.Mime);

    }

    [HttpPost]
    [DisableRequestSizeLimit]
    [Authorize(Policy = "Contributor")]
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

    [HttpPut("{id}/star")]
    public async Task<IActionResult> StarTrack([FromRoute] Guid id)
    {
        var track = await _repository.GetAsync<Track>(id, [nameof(Track.StarredBy)]);
        if (track == null)
        {
            return NotFound();
        }
        if (track.StarredBy.Any(x => x.Id == (HttpContext.Items["User"] as User)?.Id))
        {
            return Ok(ResponseUtilities.BuildWarning("The track is already starred by the user"));
        }
        track.StarredBy.Add(HttpContext.Items["User"] as User);
        await _repository.UpdateAsync(track);
        return Ok();
    }

    [HttpDelete("${id}/star")]
    public async Task<IActionResult> UnstarTrack([FromRoute] Guid id)
    {
        var track = await _repository.GetAsync<Track>(id, [nameof(Track.StarredBy)]);
        if (track == null)
        {
            return NotFound();
        }
        var user = HttpContext.Items["User"] as User;
        var starredBy = track.StarredBy.FirstOrDefault(x => x.Id == user?.Id);
        if (starredBy == null)
        {
            return Ok(ResponseUtilities.BuildWarning("The song wasn't starred by the user"));
        }

        track.StarredBy.Add(HttpContext.Items["User"] as User);
        await _repository.UpdateAsync(track);
        return Ok();
    }
}