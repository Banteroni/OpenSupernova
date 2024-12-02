using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.Data.Schemas
{
    public class ChangePasswordBody
    {
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
