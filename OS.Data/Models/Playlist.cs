using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OS.Data.Models
{
    public class Playlist : BaseModel
    {
        [MaxLength(255)]
        public required string Name { get; set; }
        [JsonIgnore]
        public User User { get; set; }
        [JsonIgnore]
        public ICollection<PlaylistTrack> PlaylistTracks { get; set; } = [];
        [JsonIgnore]
        public ICollection<StoredEntity> StoredEntities { get; set; } = [];
    }
}
