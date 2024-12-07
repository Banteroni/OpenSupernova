using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.Data.Dtos
{
    public class TracksDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public int? Duration { get; set; }
        public int Number { get; set; }
        public bool IsStarred { get; set; }
        public ICollection<Guid> ArtistIds { get; set; } = new List<Guid>();
        public Guid AlbumId { get; set; }
    }
}