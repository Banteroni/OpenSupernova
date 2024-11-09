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
    private readonly MemoryStream _data = data;
    public readonly int Width = width;
    public readonly int Height = height;

    public async void Save(string path)
    {
        try
        {
            await using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
            await _data.CopyToAsync(fileStream);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public void Dispose()
    {
        _data.Dispose();
    }
}