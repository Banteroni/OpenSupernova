using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OS.Data.Models;

public class Album : BaseModel
{
    [MaxLength(255)] public required string Name { get; set; }
    [MaxLength(255)] public string? Genre { get; set; }
    public int Year { get; set; }

    // Navigation properties
    public Artist Artist { get; set; }
    [JsonIgnore]
    public ICollection<Track> Tracks { get; set; } = new List<Track>();
}