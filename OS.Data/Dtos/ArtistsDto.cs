using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.Data.Dtos
{
    public class ArtistsDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
    }
}
