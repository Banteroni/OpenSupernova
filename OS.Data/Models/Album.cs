namespace OS.Data.Models;

public class Album : BaseModel
{
    public required string Name;
    public required string Genre;
    public int Year;
    public string? CoverPath;
    public Guid ArtistId;
    
    // Navigation properties
    public Artist Artist { get; }
    public List<Track> Tracks { get; }
}