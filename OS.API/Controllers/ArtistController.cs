using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using OS.Data.Models;
using OS.Data.Repository.Conditions;
using OS.Services.Repository;


namespace OS.API.Controllers;

[ApiController]
[Route("api/artists")]
public class ArtistController(
    IRepository repository) : Controller
{
    private readonly IRepository _repository = repository;

    [HttpGet]
    public async Task<IActionResult> GetArtists([FromQuery] [Optional] string? name)
    {
        IEnumerable<Artist> artists;
        if (name != null)
        {
            var condition = new SimpleCondition("Name", Operator.Equal, name);
            artists = await _repository.GetListAsync<Artist>(condition);
        }

        artists = await _repository.GetListAsync<Artist>();
        return Ok(artists);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetArtist([FromRoute] Guid id)
    {
        var artist = await _repository.GetAsync<Artist>(id);
        if (artist == null)
        {
            return NotFound();
        }

        return Ok(artist);
    }

    [HttpGet("{id}/albums")]
    public async Task<IActionResult> GetArtistAlbums([FromRoute] Guid id)
    {
        var artist = await _repository.GetAsync<Artist>(id);
        if (artist == null)
        {
            return NotFound();
        }

        return Ok(artist.Albums);
    }

    [HttpGet("{id}/tracks")]
    public async Task<IActionResult> GetArtistTracks([FromRoute] Guid id)
    {
        var artist = await _repository.GetAsync<Artist>(id);
        if (artist == null)
        {
            return NotFound();
        }

        return Ok(artist.Tracks);
    }
}