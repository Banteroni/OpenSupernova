using OS.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.Data.Schemas
{
    public class CreateUserBody
    {
        public required string Username { get; set; }
        public required Role Role { get; set; }
    }
}