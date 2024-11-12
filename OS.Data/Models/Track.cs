namespace OS.Data.Models;

public class Track : BaseModel
{
    public required string Name { get; set; }
    public int? Duration { get; set; }
    public int? Number { get; set; }
    public string? FileObject;
    public string? TranscodeObject;
    public Guid AlbumId;

    // Navigation properties
    public Album Album { get; }
}