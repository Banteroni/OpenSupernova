using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OS.Data.Models;

public class Track : BaseModel
{
    [MaxLength(255)] public required string Name { get; set; }
    public int? Duration { get; set; }
    public int Number { get; set; }
    [NotMapped][JsonIgnore] public Guid? NavigationAlbumId { get; set; }

    // Navigation properties
    public Album? Album { get; set; }
    public ICollection<Artist> Artists { get; set; } = [];
    public ICollection<Playlist> Playlists { get; set; } = [];
    [JsonIgnore]
    public ICollection<StoredEntity> StoredEntities { get; set; } = new List<StoredEntity>();
}