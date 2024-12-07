using OS.Data.Models;
using OS.Services.Repository;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OS.Data.Schemas;
using OS.API.Utilities;

namespace OS.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/playlists")]
    public class PlaylistController : Controller
    {
        private IRepository _repository;

        public PlaylistController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetPlaylists()
        {
            var userId = RequestUtilities.GetUserId(HttpContext);
            var playlists = await _repository.FindAllAsync<Playlist>(x => x.User.Id == userId);
            return Ok(playlists);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlaylist([FromRoute] Guid id)
        {
            var userId = RequestUtilities.GetUserId(HttpContext);
            var playlists = await _repository.FindFirstAsync<Playlist>(x => x.Id == id && x.User.Id == userId);
            if (playlists == null)
            {
                return NotFound();
            }
            return Ok(playlists);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlaylist([FromBody] CreatePlaylistBody body)
        {
            var userId = RequestUtilities.GetUserId(HttpContext);
            var user = await _repository.GetAsync<User>(userId);

            var playlist = new Playlist
            {
                Name = body.Name,
                User = user!

            };
            if (body.Tracks != null)
            {
                var tracks = await _repository.FindAllAsync<Track>(x => body.Tracks.Contains(x.Id));
                playlist.PlaylistTracks = tracks.Select((t, i) => new PlaylistTrack { Track = t, Number = i + 1 }).ToList();
            }
            await _repository.CreateAsync(playlist);
            return Ok(playlist);
        }

        [HttpPut("{id}/{trackId}")]
        public async Task<IActionResult> AddToPlaylist(Guid id, Guid trackId)
        {
            var owningPlaylist = await GetOwningPlaylist(id);
            if (owningPlaylist == null)
            {
                return NotFound();
            }
            var playlistTracks = await _repository.FindAllAsync<PlaylistTrack>(x => x.Playlist.Id == id, [nameof(PlaylistTrack.Track)]);

            if (playlistTracks.Any(x => x.Track.Id == trackId))
            {
                return BadRequest(ResponseUtilities.BuildError("The track is already present in the playlist"));
            }
            var track = await _repository.GetAsync<Track>(trackId);
            if (track == null)
            {
                return NotFound();
            }
            var playlistTrack = new PlaylistTrack
            {
                Playlist = owningPlaylist,
                Track = track,
                Number = playlistTracks.Count() == 0 ? 1 : playlistTracks.Max(x => x.Number) + 1
            };
            await _repository.CreateAsync(playlistTrack);
            return Ok(track);
        }

        [HttpDelete("{id}/{trackId}")]
        public async Task<IActionResult> RemoveFromPlaylist(Guid id, Guid trackId)
        {
            var owningPlaylist = await GetOwningPlaylist(id);
            if (owningPlaylist == null)
            {
                return NotFound();
            }
            var deletedTrack = await _repository.DeleteWhereAsync<PlaylistTrack>(x => x.Playlist.Id == id && x.Track.Id == trackId);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlaylist(Guid id)
        {
            var owningPlaylist = await GetOwningPlaylist(id);
            if (owningPlaylist == null)
            {
                return NotFound();
            }
            await _repository.DeleteAsync<Playlist>(id);
            return Ok();
        }

        private async Task<Playlist?> GetOwningPlaylist(Guid id)
        {
            var userId = RequestUtilities.GetUserId(HttpContext);
            var playlist = await _repository.FindFirstAsync<Playlist>(x => x.User.Id == userId && x.Id == id);
            return playlist;
        }
    }
}