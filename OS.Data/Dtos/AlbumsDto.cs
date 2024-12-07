using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.Data.Dtos
{
    public class AlbumsDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Genre { get; set; }
        public int Year { get; set; }
        public Guid ArtistId { get; set; }
    }
}
