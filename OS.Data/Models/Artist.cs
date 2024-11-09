namespace OS.Data.Models;

public class Artist : BaseModel
{
    public required string Name;
    public string? Bio;
    public string? ImagePath;
    
    // Navigation properties
    public List<Album> Albums { get; }
    public List<Track> Tracks { get; }
}