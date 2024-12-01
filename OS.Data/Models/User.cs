using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OS.Data.Models
{
    public enum Role
    {
        Admin,
        Contributor,
        User
    }
    public class User : BaseModel
    {
        public required string Username { get; set; }
        [JsonIgnore]
        public required string Password { get; set; }
        public Role Role { get; set; }
        [JsonIgnore]
        public ICollection<Playlist> Playlists { get; } = [];
    }
}
