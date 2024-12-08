using OS.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.Data.Dtos
{
    public class PayloadTrackDto
    {
        public string? Name { get; set; }
        public int? Duration { get; set; }
        public int? Number { get; set; }
        public List<Guid>? ArtistIds { get; set; }
        public Guid? AlbumId { get; set; }
    }
}