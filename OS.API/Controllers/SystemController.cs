using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OS.Data.Models;
using OS.Data.Schemas;
using OS.Services.Repository;
using System.Security.Cryptography;
using System.Text;

namespace OS.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/system")]
    public class SystemController : ControllerBase
    {
        IRepository _repository;
        public SystemController(IRepository repository)
        {
            _repository = repository;
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("init")]
        public async Task<IActionResult> InitSystem([FromBody] SystemInitBody body)
        {
            var users = await _repository.FindAllAsync<User>(x => x.Role == Role.Admin);
            if (users.Any())
            {
                return BadRequest("System already initialized");
            }
            // to sha256
            using var sha256 = SHA256.Create();
            var hashedPassword = sha256.ComputeHash(Encoding.UTF8.GetBytes(body.Password));
            var hashedPasswordString = Encoding.UTF8.GetString(hashedPassword);
            var user = new User
            {
                Username = "admin",
                Password = hashedPasswordString,
                Role = Role.Admin
            };
            await _repository.CreateAsync(user);
            return Ok();
        }
    }
}
