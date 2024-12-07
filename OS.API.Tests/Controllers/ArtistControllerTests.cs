using Microsoft.AspNetCore.Mvc;
using Moq;
using OS.API.Controllers;
using OS.Data.Dtos;
using OS.Data.Models;
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

        [SetUp]
        public void Setup()
        {
            _repository = new Mock<IRepository>();
            _artistController = new ArtistController(_repository.Object);
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
            Assert.IsNotNull(result);
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
            Assert.IsNotNull(result);
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
            Assert.IsNotNull(result);
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
            Assert.IsNotNull(result);
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
