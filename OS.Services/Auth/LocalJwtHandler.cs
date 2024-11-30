using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OS.Services.Auth
{
    public class LocalJwtHandler
    {
        private readonly Aes _aes;
        public SymmetricSecurityKey Key => new(_aes.Key);
        public LocalJwtHandler()
        {
            _aes = Aes.Create();
            _aes.GenerateKey();
        }
    }
}