using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OS.API.Controllers;
using OS.Data.Models;
using OS.Data.Schemas;
using OS.Services.Repository;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OS.API.Tests.Controllers
{
    public class UserControllerTests : IDisposable
    {
        private Aes _aes;
        private Mock<IRepository> _repository;
        private UserController _controller;

        [SetUp]
        public void Setup()
        {
            _repository = new Mock<IRepository>();
            _aes = Aes.Create();
            _aes.GenerateKey();

            _controller = new UserController(_repository.Object, _aes);
        }

        [Test]
        public async Task Login_WhenUserNotFound_ReturnsUnauthorized()
        {
            // Arrange
            _repository.Setup(x => x.FindAllAsync<User>(It.IsAny<Expression<Func<User, bool>>>(), null)).ReturnsAsync(new List<User>());
            // Act
            var result = await _controller.Login(new LoginBody() { Password = "Test", Username = "Test" });
            // Assert
            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        [Test]
        public async Task Login_WhenPasswordIsIncorrect_ReturnsUnauthorized()
        {
            // Arrange
            _repository.Setup(x => x.FindAllAsync<User>(It.IsAny<Expression<Func<User, bool>>>(), null)).ReturnsAsync(new List<User>() { new User() { Password = "Prova", Username = "Test" } });
            // Act
            var result = await _controller.Login(new LoginBody() { Password = "Test", Username = "Test" });
            // Assert
            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        [Test]
        public async Task Login_WhenUserFound_ReturnsToken()
        {
            // Arrange
            var sha256 = SHA256.Create();
            var hashedPassword = sha256.ComputeHash(Encoding.UTF8.GetBytes("Test"));
            var stringHashedPassword = Encoding.UTF8.GetString(hashedPassword);
            _repository.Setup(x => x.FindAllAsync<User>(It.IsAny<Expression<Func<User, bool>>>(), null)).ReturnsAsync(new List<User>() { new User() { Password = stringHashedPassword, Username = "Test" } });
            // Act
            var result = await _controller.Login(new LoginBody() { Password = "Test", Username = "Test" });
            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task ChangePassword_DifferentPassword()
        {
            // Arrange
            var sha256 = SHA256.Create();
            var hashedPassword = sha256.ComputeHash(Encoding.UTF8.GetBytes("WrongPassword"));
            var stringHashedPassword = Encoding.UTF8.GetString(hashedPassword);
            var user = new User()
            {
                Password = stringHashedPassword,
                Username = "Test"
            };
            _repository.Setup(x => x.FindAllAsync<User>(It.IsAny<Expression<Func<User, bool>>>(), null)).ReturnsAsync(new List<User>() { user });
            var httpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.PrimarySid, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, Role.Admin.ToString())
                }))
            };
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };
            // Act
            var result = await _controller.ChangePassword(new ChangePasswordBody() { CurrentPassword = "Test", NewPassword = "Test" });
            // Assert
            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        [Test]
        public async Task ChangePassword()
        {
            // Arrange
            // Arrange
            var sha256 = SHA256.Create();
            var hashedPassword = sha256.ComputeHash(Encoding.UTF8.GetBytes("Test"));
            var stringHashedPassword = Encoding.UTF8.GetString(hashedPassword);
            var user = new User()
            {
                Password = stringHashedPassword,
                Username = "Test"
            };
            _repository.Setup(x => x.FindAllAsync<User>(It.IsAny<Expression<Func<User, bool>>>(), null)).ReturnsAsync(new List<User>() { user });
            _repository.Setup(x => x.GetAsync<User>(It.IsAny<Guid>(), null)).ReturnsAsync(user);
            var httpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.PrimarySid, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, Role.Admin.ToString())
                }))
            };
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };
            // Act
            var result = await _controller.ChangePassword(new ChangePasswordBody() { CurrentPassword = "Test", NewPassword = "Test" });
            // Assert
            Assert.That(result, Is.InstanceOf<OkResult>());
        }

        [Test]
        public async Task CreateUser_Conflict()
        {
            // Arrange
            var user = new User()
            {
                Password = "Test",
                Username = "Test"
            };
            _repository.Setup(x => x.FindAllAsync<User>(It.IsAny<Expression<Func<User, bool>>>(), null)).ReturnsAsync(new List<User>() { user });
            // Act
            var result = await _controller.CreateUser(new CreateUserBody() { Role = Role.Admin, Username = "Test" });
            // Assert
            Assert.That(result, Is.InstanceOf<ConflictResult>());
        }

        [Test]
        public async Task CreateUser()
        {
            // Arrange
            var user = new User()
            {
                Password = "Test",
                Username = "Test"
            };
            _repository.Setup(x => x.FindAllAsync<User>(It.IsAny<Expression<Func<User, bool>>>(), null)).ReturnsAsync(new List<User>());
            _repository.Setup(x => x.CreateAsync<User>(It.IsAny<User>(), true)).ReturnsAsync(user);
            // Act
            var result = await _controller.CreateUser(new CreateUserBody() { Role = Role.Admin, Username = "Test" });
            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [TearDown]
        public void Dispose()
        {
            _aes.Dispose();
            _controller.Dispose();
        }
    }
}
