using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OS.Data.Models
{
    public class PlaylistTrack : BaseModel
    {
        public int Number { get; set; }

        public Track Track { get; set; }
        [JsonIgnore]
        public Playlist Playlist { get; set; }
    }
}
