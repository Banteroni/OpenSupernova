using System.Linq.Expressions;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OS.API.Utilities;
using OS.Data.Dtos;
using OS.Data.Models;
using OS.Services.Mappers;
using OS.Services.Repository;
using OS.Services.Storage;

namespace OS.API.Controllers;

[Authorize]
[ApiController]
[Route("api/albums")]
public class AlbumController(IRepository repository, IStorageService storageService, IDtoMapper mapper) : Controller
{
    private readonly IStorageService _storageService = storageService;
    private readonly IRepository _repository = repository;
    private readonly IDtoMapper _mapper = mapper;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AlbumsDto>), 200)]
    public async Task<IActionResult> GetAlbums([FromQuery][Optional] string? title, [FromQuery][Optional] int? year, [FromQuery][Optional] Guid? artistId)
    {
        Expression<Func<Album, bool>> filter = x =>
            (title == null || x.Name.Contains(title, StringComparison.OrdinalIgnoreCase)) &&
            (year == null || x.Year == year) &&
            (artistId == null || x.Artist.Id == artistId);

        var albums = await _repository.FindAllAsync<Album, AlbumsDto>(filter, [nameof(Album.Artist)]);
        return Ok(albums);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AlbumDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetAlbum([FromRoute] Guid id)
    {
        var album = await _repository.GetAsync<Album, AlbumDto>(id);
        if (album == null)
        {
            return NotFound();
        }
        return Ok(album);
    }

    [HttpPost]
    [ProducesResponseType(typeof(AlbumDto), 200)]
    [ProducesResponseType(400)]
    [Authorize(Policy = "Contributor")]
    public async Task<IActionResult> PostAlbum([FromBody] PayloadAlbumDto payload)
    {
        if (payload.ArtistId == null)
        {
            return BadRequest(ResponseUtilities.BuildError("No artist provided"));
        }
        var artist = await _repository.GetAsync<Artist>((Guid)payload.ArtistId);
        if (artist == null)
        {
            return NotFound(ResponseUtilities.BuildError("Artist not found"));
        }
        var album = _mapper.Map<Album>(payload);
        album.Artist = artist;
        await _repository.CreateAsync(album);
        return Ok(_mapper.Map<AlbumDto>(album));
    }

    [HttpPatch("{id}")]
    [Authorize(Policy = "Contributor")]
    [ProducesResponseType(typeof(AlbumDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> PatchAlbum([FromRoute] Guid id, [FromBody] PayloadAlbumDto payload)
    {
        var album = await _repository.GetAsync<Album>(id);
        if (album == null)
        {
            return NotFound();
        }
        if (payload.Name != null)
        {
            album.Name = payload.Name;
        }
        if (payload.Genre != null)
        {
            album.Genre = payload.Genre;
        }
        if (payload.Year != null)
        {
            album.Year = (int)payload.Year;
        }
        if (payload.ArtistId != null)
        {
            var artistInDb = await _repository.GetAsync<Artist>((Guid)payload.ArtistId);
            if (artistInDb == null)
            {
                return NotFound(ResponseUtilities.BuildError("Artist not found"));
            }
            album.Artist = artistInDb;
        }
        await _repository.UpdateAsync(album);
        return Ok(_mapper.Map<AlbumDto>(album));

    }

    [HttpGet("{id}/cover")]
    [ProducesResponseType(typeof(File), 200)]
    public async Task<IActionResult> GetAlbumCover([FromRoute] Guid id)
    {
        var album = await _repository.GetAsync<Album>(id, [nameof(Album.StoredEntities)]);
        if (album == null)
        {
            return NotFound(ResponseUtilities.BuildError("Album not found"));
        }
        var cover = album.StoredEntities.FirstOrDefault(x => x.Type == StoredEntityType.AlbumCover);
        if (cover == null)
        {
            return NotFound(ResponseUtilities.BuildError("Album cover not found"));
        }
        var stream = await _storageService.GetFileStream(cover.Id.ToString());
        if (stream == null)
        {
            return Problem();
        }
        return File(stream, cover.Mime);
    }
}