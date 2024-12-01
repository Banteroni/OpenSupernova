using OS.Data.Models;
using OS.Data.Repository.Conditions;
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
            var user = HttpContext.Items["User"] as OS.Data.Models.User;
            var playlists = await _repository.GetListAsync<Playlist>(new SimpleCondition(nameof(OS.Data.Models.User.Id), Operator.Equal, user.Id, nameof(User)));
            return Ok(playlists);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlaylist([FromRoute] Guid id)
        {
            var user = HttpContext.Items["User"] as OS.Data.Models.User;
            var compositeCondition = new CompositeCondition(LogicalSwitch.And);
            compositeCondition.AddCondition(new SimpleCondition(nameof(Playlist.Id), Operator.Equal, id));
            compositeCondition.AddCondition(new SimpleCondition(nameof(OS.Data.Models.User.Id), Operator.Equal, user.Id, nameof(OS.Data.Models.User)));
            var playlists = await _repository.GetListAsync<Playlist>(compositeCondition);
            if (playlists.Count() > 0)
            {
                return Ok(playlists.First());
            }
            return NotFound();
        }

        [HttpGet("{id}/tracks")]
        public async Task<IActionResult> GetPlaylistTracks([FromRoute] Guid id)
        {
            var owningPlaylist = await GetOwningPlaylist(id);
            if (owningPlaylist == null)
            {
                return NotFound();
            }
            var playlistTracks = await _repository.GetListAsync<PlaylistTrack>(new SimpleCondition(nameof(Playlist.Id), Operator.Equal, id, nameof(Playlist)), [nameof(PlaylistTrack.Track)]);
            if (playlistTracks.Count() > 0)
            {
                return Ok(playlistTracks.OrderBy(x => x.Number).Select(x => x.Track));
            }
            return Ok(new List<Track>());
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlaylist([FromBody] CreatePlaylistBody body)
        {
            var user = HttpContext.Items["User"] as OS.Data.Models.User;

            var playlist = new Playlist
            {
                Name = body.Name,
                User = user

            };
            if (body.Tracks != null)
            {
                var tracks = await _repository.GetListAsync<Track>(body.Tracks);
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
            var compositeCondition = new CompositeCondition(LogicalSwitch.And);
            compositeCondition.AddCondition(new SimpleCondition(nameof(Playlist.Id), Operator.Equal, id, nameof(Playlist)));
            var playlistTracks = await _repository.GetListAsync<PlaylistTrack>(compositeCondition, [nameof(PlaylistTrack.Track)]);

            if (playlistTracks != null && playlistTracks.Any(x => x.Track.Id == trackId))
            {
                return BadRequest(ResponseUtilities.BuildErrorBody("The track is already present in the playlist"));
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
            var compositeCondition = new CompositeCondition(LogicalSwitch.And);
            compositeCondition.AddCondition(new SimpleCondition(nameof(Playlist.Id), Operator.Equal, id, nameof(Playlist)));
            compositeCondition.AddCondition(new SimpleCondition(nameof(Track.Id), Operator.Equal, trackId, nameof(Track)));
            var playlistTrack = (await _repository.GetListAsync<PlaylistTrack>(compositeCondition)).FirstOrDefault();
            if (playlistTrack == null)
            {
                return NotFound();
            }
            await _repository.DeleteAsync<PlaylistTrack>(playlistTrack.Id);
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
            var user = HttpContext.Items["User"] as OS.Data.Models.User;
            var condition = new SimpleCondition(nameof(OS.Data.Models.User.Id), Operator.Equal, user.Id, nameof(OS.Data.Models.User));
            var playlists = await _repository.GetListAsync<Playlist>(condition);
            return playlists.FirstOrDefault(x => x.Id == id);
        }
    }
}