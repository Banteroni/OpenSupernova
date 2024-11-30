using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OS.Data.Models;
using OS.Data.Repository.Conditions;
using OS.Services.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace OS.API.Controllers
{
    public class LoginPayload
    {
        public required string Password { get; set; }
        public required string Username { get; set; }
    }

    [ApiController]
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
        public async Task<IActionResult> Login([FromBody] LoginPayload payload)
        {
            var users = await _repository.GetListAsync<User>(new SimpleCondition(nameof(OS.Data.Models.User.Username), Operator.Equal, payload.Username));
            var user = users.FirstOrDefault();
            if (user == null)
            {
                return Unauthorized();
            }
            if (user.Password != payload.Password)
            {
                return Unauthorized();
            }

            var key = new SymmetricSecurityKey(_aes.Key);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "OpenSupernova",
                audience: "OpenSupernova",
                claims: new[] { new Claim(ClaimTypes.Name, user.Username), new Claim(ClaimTypes.Role, user.Role.ToString()) },
                expires: DateTime.Now.AddDays(30),
                signingCredentials: creds
                );
            var stringToken = new JwtSecurityTokenHandler().WriteToken(token);
            var keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("token", stringToken);

            return Ok(keyValuePairs);
        }
    }
}
