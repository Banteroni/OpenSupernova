using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.Data.Models
{
    public class Playlist
    {
        public Guid Id { get; init; }
        [MaxLength(255)]
        public required string Name { get; set; }
        public ICollection<Track> Tracks { get; } = [];
    }
}
