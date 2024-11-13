namespace OS.Data.Files;

public class MetadataPicture(
    MediaType type,
    string mimeType,
    string description,
    byte[] data,
    int width,
    int height)
{
    public readonly MediaType Type = type;
    public readonly string MimeType = mimeType;
    public readonly string Description = description;
    public readonly byte[] Data = data;
    public readonly int Width = width;
    public readonly int Height = height;
}