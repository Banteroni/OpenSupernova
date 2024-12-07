using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OS.Data.Dtos;
using OS.Data.Models;
using OS.Services.Repository;

namespace OS.API.Controllers;

[Authorize]
[ApiController]
[Route("api/artists")]
public class ArtistController(
    IRepository repository) : Controller
{
    private readonly IRepository _repository = repository;

    [HttpGet]
    public async Task<IActionResult> GetArtists([FromQuery] [Optional] string? name)
    {
        IEnumerable<ArtistsDto> artists;
        if (name != null)
        {
            artists = await _repository.FindAllAsync<Artist, ArtistsDto>(x => x.Name.Contains(name));
        }
        else
        {
            artists = await _repository.GetAllAsync<Artist, ArtistsDto>();
        }
        return Ok(artists);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetArtist([FromRoute] Guid id)
    {
        var artist = await _repository.GetAsync<Artist, ArtistDto>(id);
        if (artist == null)
        {
            return NotFound();
        }

        return Ok(artist);
    }
}