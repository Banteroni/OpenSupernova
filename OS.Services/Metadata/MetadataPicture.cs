namespace OS.Services.Metadata;

public class MetadataPicture
{
    public FlacMediaType Type { get; set; }
    public string MimeType { get; set; }
    public string Description { get; set; }
    public byte[] Data { get; set; }
}