namespace OS.Data.Models;

public class Track : BaseModel
{
     public required string Name;
     public required string Duration;
     public int Number;
     public string? FilePath;
     public Guid AlbumId;
     
     // Navigation properties
     public Album Album { get; }
     public List<Transcode> TranscodedFiles { get; }
}