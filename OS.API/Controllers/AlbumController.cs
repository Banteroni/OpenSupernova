using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using OS.Services.IRepository;

namespace OS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlbumController(IRepository repository) : Controller
{
    private readonly IRepository _repository = repository;
    [HttpGet]
    public async Task<IActionResult> GetAlbums([FromQuery][Optional] string? title, [FromQuery][Optional] int? year, [FromQuery][Optional] string? artist)
    {
        var albums = await _repository.GetAlbumsAsync(title, year, artist);
        return Ok(albums);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAlbum([FromRoute]Guid id)
    {
        var album = await _repository.GetAlbumAsync(id);
        if (album == null)
        {
            return NotFound();
        }
        return Ok(album);
    }
}