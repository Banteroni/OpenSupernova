using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OS.Data.Models;

public class Track : BaseModel
{
    [MaxLength(255)] public required string Name { get; set; }
    public int? Duration { get; set; }
    public int? Number { get; set; }
    public string? FileObject;

    [NotMapped] public Guid? NavigationAlbumId { get; set; }

    // Navigation properties
    public Album? Album { get; set; }
}