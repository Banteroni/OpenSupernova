using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using OS.Data.Models;
using OS.Data.Repository.Conditions;
using OS.Services.Repository;

namespace OS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlbumController(IRepository repository) : Controller
{
    private readonly IRepository _repository = repository;
    [HttpGet]
    public async Task<IActionResult> GetAlbums([FromQuery][Optional] string? title, [FromQuery][Optional] int? year, [FromQuery][Optional] string? artist)
    {
        var compositeCondition = new CompositeCondition(LogicalSwitch.Or);
        if (title != null)
        {
            compositeCondition.AddCondition(new SimpleCondition("Name", Operator.Contains, title));
        }
        if (year != null)
        {
            compositeCondition.AddCondition(new SimpleCondition("Year", Operator.Equal, (int)year));
        }
        if (artist != null)
        {
            compositeCondition.AddCondition(new SimpleCondition("Name", Operator.Contains, artist, nameof(Artist)));
        }
        
        var albums = await _repository.GetListAsync<Album>(compositeCondition, [nameof(Artist)]);
        return Ok(albums);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAlbum([FromRoute]Guid id)
    {
        var album = await _repository.GetAsync<Album>(id, [nameof(Artist)]);
        if (album == null)
        {
            return NotFound();
        }
        return Ok(album);
    }
    
    [HttpGet("{id}/tracks")]
    public async Task<IActionResult> GetAlbumTracks([FromRoute]Guid id)
    {
        var album = await _repository.GetAsync<Album>(id, new []{nameof(Album.Tracks)});
        if (album == null)
        {
            return NotFound();
        }
        return Ok(album.Tracks);
    }
}