namespace OS.Data.Files;

public class MetadataPicture(
    MediaType type,
    string mimeType,
    string description,
    MemoryStream data,
    int width,
    int height) : IDisposable
{
    public readonly MediaType Type = type;
    public readonly string MimeType = mimeType;
    public readonly string Description = description;
    public readonly MemoryStream Data = data;
    public readonly int Width = width;
    public readonly int Height = height;

    public void Dispose()
    {
        Data.Dispose();
    }
}