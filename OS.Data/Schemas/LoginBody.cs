using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.Data.Schemas
{
    public class LoginBody
    {
        public required string Password { get; set; }
        public required string Username { get; set; }
    }
}
