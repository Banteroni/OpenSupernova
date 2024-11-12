namespace OS.Data.Models;

public class Album : BaseModel
{
    public required string Name { get; set; }
    public string? Genre { get; set; }
    public int? Year { get; set; }
    public string? CoverPath { get; set; }
    public Guid ArtistId;

    // Navigation properties
    public Artist Artist { get; set; }
    public List<Track> Tracks { get; set; }
}