using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OS.API.Utilities;
using OS.Data.Dtos;
using OS.Data.Models;
using OS.Services.Mappers;
using OS.Services.Repository;

namespace OS.API.Controllers;

[Authorize]
[ApiController]
[Route("api/artists")]
public class ArtistController(
    IRepository repository, IDtoMapper mapper) : Controller
{
    private readonly IDtoMapper _mapper = mapper;
    private readonly IRepository _repository = repository;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ArtistsDto>), 200)]
    public async Task<IActionResult> GetArtists([FromQuery][Optional] string? name)
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
    [ProducesResponseType(typeof(ArtistsDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetArtist([FromRoute] Guid id)
    {
        var artist = await _repository.GetAsync<Artist, ArtistDto>(id);
        if (artist == null)
        {
            return NotFound();
        }

        return Ok(artist);
    }

    [HttpPost]
    [Authorize(Policy = "Contributor")]
    [ProducesResponseType(typeof(ArtistDto), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> PostArtist([FromBody] PayloadArtistDto artistDto)
    {
        if (artistDto.Name == null)
        {
            return BadRequest(ResponseUtilities.BuildError("No name provided"));
        }
        var artist = new Artist
        {
            Name = artistDto.Name
        };
        artist = await _repository.CreateAsync(artist);

        return Ok(artist);
    }

    [HttpPatch("{id}")]
    [Authorize(Policy = "Contributor")]
    [ProducesResponseType(typeof(ArtistDto), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> PatchArtist([FromRoute] Guid id, [FromBody] PayloadArtistDto artistDto)
    {
        var artist = await _repository.GetAsync<Artist>(id);
        if (artist == null)
        {
            return NotFound();
        }
        if (artistDto.Name != null)
        {
            artist.Name = artistDto.Name;
        }
        artist = await _repository.UpdateAsync(artist);
        return Ok(_mapper.Map<ArtistDto>(artist));
    }
}