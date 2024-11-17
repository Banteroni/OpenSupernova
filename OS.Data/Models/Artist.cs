using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OS.Data.Models;

public class Artist : BaseModel
{
    [MaxLength(255)] public required string Name { get; set; }
    [MaxLength(3096)] public string? Bio { get; set; }
    [MaxLength(255)] public string? ImagePath { get; set; }

    // Navigation properties
    [JsonIgnore]
    public ICollection<Album> Albums { get; } = new List<Album>();
    [JsonIgnore]
    public ICollection<Track> Tracks { get; } = new List<Track>();
}