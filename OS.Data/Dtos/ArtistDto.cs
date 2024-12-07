using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.Data.Dtos
{
    public class ArtistDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Bio { get; set; }
    }
}
