using System.Linq.Expressions;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OS.API.Utilities;
using OS.Data.Models;
using OS.Services.Repository;
using OS.Services.Storage;

namespace OS.API.Controllers;

[Authorize]
[ApiController]
[Route("api/albums")]
public class AlbumController(IRepository repository, IStorageService storageService) : Controller
{
    private readonly IStorageService _storageService = storageService;
    private readonly IRepository _repository = repository;
    [HttpGet]
    public async Task<IActionResult> GetAlbums([FromQuery][Optional] string? title, [FromQuery][Optional] int? year, [FromQuery][Optional] string? artist)
    {
        Expression<Func<Album, bool>> filter = x =>
            (title == null || x.Name.Contains(title, StringComparison.OrdinalIgnoreCase)) &&
            (year == null || x.Year == year) &&
            (artist == null || x.Artist.Name.Contains(artist, StringComparison.OrdinalIgnoreCase));

        var albums = await _repository.FindAllAsync(filter, [nameof(Album.Artist)]);
        return Ok(albums);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAlbum([FromRoute] Guid id)
    {
        var album = await _repository.GetAsync<Album>(id);
        if (album == null)
        {
            return NotFound();
        }
        return Ok(album);
    }

    [HttpGet("{id}/tracks")]
    public async Task<IActionResult> GetAlbumTracks([FromRoute] Guid id)
    {
        var album = await _repository.FindFirstAsync<Album>(x => x.Id == id, [nameof(Album.Tracks)]);
        if (album == null)
        {
            return NotFound();
        }
        return Ok(album.Tracks.OrderBy(x => x.Number));
    }
    [HttpGet("{id}/cover")]
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