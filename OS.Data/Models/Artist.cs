namespace OS.Data.Models;

public class Artist : BaseModel
{
    public required string Name { get; set; }
    public string? Bio { get; set; }
    public string? ImagePath { get; set; }
    
    // Navigation properties
    public List<Album> Albums { get; }
    public List<Track> Tracks { get; }
}