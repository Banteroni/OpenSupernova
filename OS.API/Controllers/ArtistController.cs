using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using OS.Services.Repository;


namespace OS.API.Controllers;

public class ArtistController(
    IRepository repository) : Controller
{
    private readonly IRepository _repository = repository;
    
    [HttpGet]
    public async Task<IActionResult> GetArtists([FromQuery][Optional] string? name)
    {
        var artists = await _repository.GetArtistsAsync(name);
        return Ok(artists);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetArtist([FromRoute]Guid id)
    {
        var artist = await _repository.GetArtistAsync(id);
        if (artist == null)
        {
            return NotFound();
        }
        return Ok(artist);
    }
    
    [HttpGet("{id}/albums")]
    public async Task<IActionResult> GetArtistAlbums([FromRoute]Guid id)
    {
        var artist = await _repository.GetArtistAsync(id);
        if (artist == null)
        {
            return NotFound();
        }
        return Ok(artist.Albums);
    }
    [HttpGet("{id}/tracks")]
    public async Task<IActionResult> GetArtistTracks([FromRoute]Guid id)
    {
        var artist = await _repository.GetArtistAsync(id);
        if (artist == null)
        {
            return NotFound();
        }
        return Ok(artist.Tracks);
    }
}
