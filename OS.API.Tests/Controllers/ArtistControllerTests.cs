using Microsoft.AspNetCore.Mvc;
using Moq;
using OS.API.Controllers;
using OS.Data.Dtos;
using OS.Data.Models;
using OS.Services.Mappers;
using OS.Services.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OS.API.Tests.Controllers
{
    public class ArtistControllerTests : IDisposable
    {
        public ArtistController _artistController;
        public Mock<IRepository> _repository;
        public IDtoMapper _mapper;

        [SetUp]
        public void Setup()
        {
            _mapper = new DtoMapper();
            _repository = new Mock<IRepository>();
            _artistController = new ArtistController(_repository.Object, _mapper);
        }

        [Test]
        public void GetArtists_WithName()
        {
            // Arrange
            var artist = new Artist()
            {
                Name = "Test"
            };
            var artistDto = new ArtistsDto()
            {
                Name = "Test"
            };
            var artists = new List<Artist> { artist };
            var artistsDto = new List<ArtistsDto> { artistDto };
            _repository.Setup(x => x.FindAllAsync<Artist, ArtistsDto>(It.IsAny<Expression<Func<Artist, bool>>>(), null)).ReturnsAsync(artistsDto);

            // Act
            var result = _artistController.GetArtists("Test").Result as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.Value, Is.EqualTo(artistsDto));
        }

        [Test]
        public void GetAllArtists()
        {
            // Arrange
            var artist = new Artist()
            {
                Name = "Test"
            };
            var artistDto = new ArtistsDto()
            {
                Name = "Test"
            };
            var artists = new List<Artist> { artist };
            var artistsDto = new List<ArtistsDto> { artistDto };
            _repository.Setup(x => x.GetAllAsync<Artist, ArtistsDto>(null)).ReturnsAsync(artistsDto);

            // Act
            var result = _artistController.GetArtists(null).Result as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.Value, Is.EqualTo(artistsDto));
        }

        [Test]
        public void GetArtist()
        {
            // Arrange
            var artist = new Artist()
            {
                Name = "Test"
            };
            var artistDto = new ArtistDto()
            {
                Name = "Test"
            };
            _repository.Setup(x => x.GetAsync<Artist, ArtistDto>(It.IsAny<Guid>(), null)).ReturnsAsync(artistDto);
            // Act
            var result = _artistController.GetArtist(Guid.NewGuid()).Result as OkObjectResult;
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.Value, Is.EqualTo(artistDto));
        }

        [Test]
        public void GetArtistNotFound()
        {
            // Arrange
            _repository.Setup(x => x.GetAsync<Artist, ArtistDto>(It.IsAny<Guid>(), null)).ReturnsAsync((ArtistDto)null);
            // Act
            var result = _artistController.GetArtist(Guid.NewGuid()).Result as NotFoundResult;
            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void PostArtist()
        {
            // Arrange
            var artistDto = new PayloadArtistDto()
            {
                Name = "Test"
            };
            var artist = new Artist()
            {
                Name = "Test"
            };
            _repository.Setup(x => x.CreateAsync(It.IsAny<Artist>(), It.IsAny<bool>())).ReturnsAsync(artist);
            // Act
            var result = _artistController.PostArtist(artistDto).Result as OkObjectResult;
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.Value, Is.EqualTo(artist));
        }

        [Test]
        public void PostArtistNoName()
        {
            // Arrange
            var artistDto = new PayloadArtistDto()
            {
                Name = null
            };
            // Act
            var result = _artistController.PostArtist(artistDto).Result as BadRequestObjectResult;
            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void PatchArtistNotFound()
        {
            // Arrange
            _repository.Setup(x => x.GetAsync<Artist>(It.IsAny<Guid>(), null)).ReturnsAsync((Artist)null);
            // Act
            var result = _artistController.PatchArtist(Guid.NewGuid(), new PayloadArtistDto()).Result as NotFoundResult;
            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void PatchArtistNoName()
        {
            // Arrange
            var artist = new Artist()
            {
                Name = "Test"
            };
            var artistDto = new PayloadArtistDto()
            {
                Name = null
            };
            _repository.Setup(x => x.GetAsync<Artist>(It.IsAny<Guid>(), null)).ReturnsAsync(artist);
            // Act
            var result = _artistController.PatchArtist(Guid.NewGuid(), artistDto).Result as OkObjectResult;
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(artist.Name, Is.EqualTo("Test"));
        }

        [Test]
        public void PatchArtist()
        {
            // Arrange
            var artist = new Artist()
            {
                Name = "Test"
            };
            var artistDto = new PayloadArtistDto()
            {
                Name = "Test2"
            };
            _repository.Setup(x => x.GetAsync<Artist>(It.IsAny<Guid>(), null)).ReturnsAsync(artist);
            // Act
            var result = _artistController.PatchArtist(Guid.NewGuid(), artistDto).Result as OkObjectResult;
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(artist.Name, Is.EqualTo("Test2"));
        }

        [TearDown]
        public void TearDown()
        {
            _artistController.Dispose();
        }

        public void Dispose()
        {
            _artistController?.Dispose();
        }
    }
}
