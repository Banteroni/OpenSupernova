using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OS.API.Controllers;
using OS.Data.Models;
using OS.Data.Schemas;
using OS.Services.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OS.API.Tests.Controllers
{
    public class PlaylistControllerTests : IDisposable
    {
        public PlaylistController _playlistController;
        public Mock<IRepository> _repository;

        [SetUp]
        public void Setup()
        {
            _repository = new Mock<IRepository>();
            _playlistController = new PlaylistController(_repository.Object);
        }

        [Test]
        public void GetPlaylists()
        {
            // Arrange
            var user = new User()
            {
                Username = "Test",
                Password = "Test",
                Id = Guid.NewGuid()
            };
            _playlistController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.PrimarySid, user.Id.ToString())
                    }))
                }
            };

            var playlist = new Playlist()
            {
                Name = "Test",
                User = user
            };
            var playlists = new List<Playlist> { playlist };
            _repository.Setup(x => x.FindAllAsync<Playlist>(It.IsAny<Expression<Func<Playlist, bool>>>(), null)).ReturnsAsync(playlists);

            // Act
            var result = _playlistController.GetPlaylists().Result as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);

        }

        [Test]
        public void GetPlaylist()
        {
            // Arrange
            var user = new User()
            {
                Username = "Test",
                Password = "Test",
                Id = Guid.NewGuid()
            };
            _playlistController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.PrimarySid, user.Id.ToString())
                    }))
                }
            };
            var playlist = new Playlist()
            {
                Name = "Test",
                User = user
            };
            _repository.Setup(x => x.FindFirstAsync<Playlist>(It.IsAny<Expression<Func<Playlist, bool>>>(), null)).ReturnsAsync(playlist);

            // Act
            var result = _playlistController.GetPlaylist(Guid.NewGuid()).Result as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetPlaylist_NotFound()
        {
            // Arrange
            var user = new User()
            {
                Username = "Test",
                Password = "Test",
                Id = Guid.NewGuid()
            };
            _playlistController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.PrimarySid, user.Id.ToString())
                    }))
                }
            };
            _repository.Setup(x => x.FindFirstAsync<Playlist>(It.IsAny<Expression<Func<Playlist, bool>>>(), null)).ReturnsAsync((Playlist)null);
            // Act
            var result = _playlistController.GetPlaylist(Guid.NewGuid()).Result as NotFoundResult;
            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void CreatePlaylist()
        {
            // Arrange
            var user = new User()
            {
                Username = "Test",
                Password = "Test",
                Id = Guid.NewGuid()
            };
            _playlistController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.PrimarySid, user.Id.ToString())
                    }))
                }
            };
            var playlist = new Playlist()
            {
                Name = "Test",
                User = user
            };
            var playlistBody = new CreatePlaylistBody()
            {
                Name = "Test"
            };
            _repository.Setup(x => x.GetAsync<User>(It.IsAny<Guid>(), null)).ReturnsAsync(user);
            _repository.Setup(x => x.CreateAsync(playlist, true)).ReturnsAsync(playlist);
            // Act
            var result = _playlistController.CreatePlaylist(playlistBody).Result as OkObjectResult;
            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void AddToPlaylist()
        {
            // Arrange
            var user = new User()
            {
                Username = "Test",
                Password = "Test",
                Id = Guid.NewGuid()
            };
            _playlistController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.PrimarySid, user.Id.ToString())
                    }))
                }
            };
            var playlist = new Playlist()
            {
                Name = "Test",
                User = user,
                PlaylistTracks = new List<PlaylistTrack>()
            };
            var playlistTrack = new PlaylistTrack()
            {
                Playlist = playlist,
                Track = new Track() { Name = "Test" }
            };
            var playlistTracks = new List<PlaylistTrack> { playlistTrack };
            _repository.Setup(x => x.FindFirstAsync<Playlist>(It.IsAny<Expression<Func<Playlist, bool>>>(), null)).ReturnsAsync(playlist);
            _repository.Setup(x => x.FindAllAsync<PlaylistTrack>(It.IsAny<Expression<Func<PlaylistTrack, bool>>>(), new string[] { nameof(PlaylistTrack.Track) })).ReturnsAsync(playlistTracks);
            _repository.Setup(x => x.GetAsync<Track>(It.IsAny<Guid>(), null)).ReturnsAsync(playlistTrack.Track);
            // Act
            var result = _playlistController.AddToPlaylist(Guid.NewGuid(), Guid.NewGuid()).Result as OkObjectResult;
            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void AddToPlaylist_PlaylistNotFound()
        {
            // Arrange
            var user = new User()
            {
                Username = "Test",
                Password = "Test",
                Id = Guid.NewGuid()
            };
            _playlistController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.PrimarySid, user.Id.ToString())
                    }))
                }
            };
            _repository.Setup(x => x.FindFirstAsync<Playlist>(It.IsAny<Expression<Func<Playlist, bool>>>(), null)).ReturnsAsync((Playlist)null);
            // Act
            var result = _playlistController.AddToPlaylist(Guid.NewGuid(), Guid.NewGuid()).Result as NotFoundResult;
            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void AddToPlaylist_TrackNotFound()
        {
            // Arrange
            var user = new User()
            {
                Username = "Test",
                Password = "Test",
                Id = Guid.NewGuid()
            };
            _playlistController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.PrimarySid, user.Id.ToString())
                    }))
                }
            };
            var playlist = new Playlist()
            {
                Name = "Test",
                User = user,
                PlaylistTracks = new List<PlaylistTrack>()
            };
            var playlistTrack = new PlaylistTrack()
            {
                Playlist = playlist,
                Track = new Track() { Name = "Test" }
            };
            var playlistTracks = new List<PlaylistTrack> { playlistTrack };
            _repository.Setup(x => x.FindFirstAsync<Playlist>(It.IsAny<Expression<Func<Playlist, bool>>>(), null)).ReturnsAsync(playlist);
            _repository.Setup(x => x.FindAllAsync<PlaylistTrack>(It.IsAny<Expression<Func<PlaylistTrack, bool>>>(), new string[] { nameof(PlaylistTrack.Track) })).ReturnsAsync(playlistTracks);
            _repository.Setup(x => x.GetAsync<Track>(It.IsAny<Guid>(), null)).ReturnsAsync((Track)null);
            // Act
            var result = _playlistController.AddToPlaylist(Guid.NewGuid(), Guid.NewGuid()).Result as NotFoundResult;
            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void AddToPlaylist_AlreadyInPlaylist()
        {
            // Arrange
            var user = new User()
            {
                Username = "Test",
                Password = "Test",
                Id = Guid.NewGuid()
            };
            _playlistController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.PrimarySid, user.Id.ToString())
                    }))
                }
            };
            var playlist = new Playlist()
            {
                Name = "Test",
                User = user,
                PlaylistTracks = new List<PlaylistTrack>()
            };
            var playlistTrack = new PlaylistTrack()
            {
                Playlist = playlist,
                Track = new Track() { Name = "Test" }
            };
            var playlistTracks = new List<PlaylistTrack> { playlistTrack };
            _repository.Setup(x => x.FindFirstAsync<Playlist>(It.IsAny<Expression<Func<Playlist, bool>>>(), null)).ReturnsAsync(playlist);
            _repository.Setup(x => x.FindAllAsync<PlaylistTrack>(It.IsAny<Expression<Func<PlaylistTrack, bool>>>(), new string[] { nameof(PlaylistTrack.Track) })).ReturnsAsync(playlistTracks);
            _repository.Setup(x => x.GetAsync<Track>(It.IsAny<Guid>(), null)).ReturnsAsync(playlistTrack.Track);
            // Act
            var result = _playlistController.AddToPlaylist(Guid.NewGuid(), playlistTrack.Track.Id).Result as BadRequestObjectResult;
            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void RemoveFromPlaylist()
        {
            // Arrange
            var user = new User()
            {
                Username = "Test",
                Password = "Test",
                Id = Guid.NewGuid()
            };
            _playlistController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.PrimarySid, user.Id.ToString())
                    }))
                }
            };
            var playlist = new Playlist()
            {
                Name = "Test",
                User = user,
                PlaylistTracks = new List<PlaylistTrack>()
            };
            var playlistTrack = new PlaylistTrack()
            {
                Playlist = playlist,
                Track = new Track() { Name = "Test" }
            };
            var playlistTracks = new List<PlaylistTrack> { playlistTrack };
            _repository.Setup(x => x.FindFirstAsync<Playlist>(It.IsAny<Expression<Func<Playlist, bool>>>(), null)).ReturnsAsync(playlist);
            _repository.Setup(x => x.FindAllAsync<PlaylistTrack>(It.IsAny<Expression<Func<PlaylistTrack, bool>>>(), null)).ReturnsAsync(playlistTracks);
            // Act
            var result = _playlistController.RemoveFromPlaylist(Guid.NewGuid(), Guid.NewGuid()).Result as OkResult;
            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void RemoveFromPlaylist_PlaylistNotFound()
        {
            // Arrange
            var user = new User()
            {
                Username = "Test",
                Password = "Test",
                Id = Guid.NewGuid()
            };
            _playlistController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.PrimarySid, user.Id.ToString())
                    }))
                }
            };
            _repository.Setup(x => x.FindFirstAsync<Playlist>(It.IsAny<Expression<Func<Playlist, bool>>>(), null)).ReturnsAsync((Playlist)null);
            // Act
            var result = _playlistController.RemoveFromPlaylist(Guid.NewGuid(), Guid.NewGuid()).Result as NotFoundResult;
            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void DeletePlaylist()
        {
            // Arrange
            var user = new User()
            {
                Username = "Test",
                Password = "Test",
                Id = Guid.NewGuid()
            };
            _playlistController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.PrimarySid, user.Id.ToString())
                    }))
                }
            };
            var playlist = new Playlist()
            {
                Name = "Test",
                User = user
            };
            _repository.Setup(x => x.FindFirstAsync<Playlist>(It.IsAny<Expression<Func<Playlist, bool>>>(), null)).ReturnsAsync(playlist);
            // Act
            var result = _playlistController.DeletePlaylist(Guid.NewGuid()).Result as OkResult;
            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void DeletePlaylist_NotFound()
        {
            // Arrange
            var user = new User()
            {
                Username = "Test",
                Password = "Test",
                Id = Guid.NewGuid()
            };
            _playlistController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.PrimarySid, user.Id.ToString())
                    }))
                }
            };
            _repository.Setup(x => x.FindFirstAsync<Playlist>(It.IsAny<Expression<Func<Playlist, bool>>>(), null)).ReturnsAsync((Playlist)null);
            // Act
            var result = _playlistController.DeletePlaylist(Guid.NewGuid()).Result as NotFoundResult;
            // Assert
            Assert.That(result, Is.Not.Null);
        }



        [TearDown]
        public void TearDown()
        {
            _playlistController.Dispose();
        }

        public void Dispose()
        {
            _playlistController?.Dispose();
        }
    }
}