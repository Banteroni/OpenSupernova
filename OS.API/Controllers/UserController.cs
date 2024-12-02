using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OS.Data.Models;
using OS.Data.Schemas;
using OS.Services.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OS.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/users")]
    public class UserController : Controller
    {
        private IRepository _repository;
        private Aes _aes;

        public UserController(IRepository repository, Aes aes)
        {
            _repository = repository;
            _aes = aes;

        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginBody payload)
        {
            var users = await _repository.FindAllAsync<User>(x => x.Username == payload.Username);
            var user = users.FirstOrDefault();
            if (user == null)
            {
                return Unauthorized();
            }

            using var sha256 = SHA256.Create();
            var hashedPassword = sha256.ComputeHash(Encoding.UTF8.GetBytes(payload.Password));
            var hashedPasswordString = Encoding.UTF8.GetString(hashedPassword);
            if (user.Password != hashedPasswordString)
            {
                return Unauthorized();
            }

            var key = new SymmetricSecurityKey(_aes.Key);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "OpenSupernova",
                audience: "OpenSupernova",
                claims: new[] { new Claim(ClaimTypes.PrimarySid, user.Id.ToString()), new Claim(ClaimTypes.Role, user.Role.ToString()) },
                expires: DateTime.Now.AddDays(30),
                signingCredentials: creds
                );
            var stringToken = new JwtSecurityTokenHandler().WriteToken(token);
            var keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("token", stringToken);

            return Ok(keyValuePairs);
        }

        [HttpPatch]
        [Route("password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordBody payload)
        {
            var user = HttpContext.Items["User"] as OS.Data.Models.User;
            using var sha256 = SHA256.Create();
            var hashedPassword = sha256.ComputeHash(Encoding.UTF8.GetBytes(payload.CurrentPassword));
            var hashedPasswordString = Encoding.UTF8.GetString(hashedPassword);
            if (user?.Password != hashedPasswordString)
            {
                return Unauthorized();
            }
            var newHashedPassword = sha256.ComputeHash(Encoding.UTF8.GetBytes(payload.NewPassword));
            var newHashedPasswordString = Encoding.UTF8.GetString(newHashedPassword);
            user.Password = newHashedPasswordString;
            await _repository.UpdateAsync(user);
            return Ok();
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserBody payload)
        {
            var users = await _repository.FindAllAsync<User>(x => x.Username == payload.Username);
            if (users.Count() > 0)
            {
                return Conflict();
            }
            using var sha256 = SHA256.Create();
            var random = GenerateChar(12);
            var hashedPassword = sha256.ComputeHash(Encoding.UTF8.GetBytes(random));
            var hashedPasswordString = Encoding.UTF8.GetString(hashedPassword);
            var user = new User
            {
                Username = payload.Username,
                Password = hashedPasswordString,
                Role = payload.Role
            };
            await _repository.CreateAsync(user);
            var response = new Dictionary<string, string>();
            response.Add("password", random);
            return Ok(response);
        }
        private string GenerateChar()
        {
            Random random = new Random();

            return Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65))).ToString();
        }

        private string GenerateChar(int count)
        {
            string randomString = "";

            for (int i = 0; i < count; i++)
            {
                randomString += GenerateChar();
            }

            return randomString;
        }
    }
}
