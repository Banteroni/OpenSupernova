using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OS.Data.Models;

public class Track : BaseModel
{
    [MaxLength(255)] public required string Name { get; set; }
    public int? Duration { get; set; }
    public int Number { get; set; }
    // Navigation properties
    public Album? Album { get; set; }
    public ICollection<Artist> Artists { get; set; } = [];
    [JsonIgnore]
    public ICollection<Playlist> Playlists { get; set; } = [];
    [JsonIgnore]
    public ICollection<StoredEntity> StoredEntities { get; set; } = new List<StoredEntity>();
    [JsonIgnore]
    public ICollection<User> StarredBy { get; set; } = [];
}