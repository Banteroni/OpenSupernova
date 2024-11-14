using System.ComponentModel.DataAnnotations;

namespace OS.Data.Models;

public class Artist : BaseModel
{
    [MaxLength(255)] public required string Name { get; set; }
    [MaxLength(3096)] public string? Bio { get; set; }
    [MaxLength(255)] public string? ImagePath { get; set; }

    // Navigation properties
    public ICollection<Album> Albums { get; } = new List<Album>();
    public ICollection<Track> Tracks { get; } = new List<Track>();
}