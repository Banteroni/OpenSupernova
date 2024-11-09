namespace OS.Data.Files;

public abstract class BaseFile(FileStream data) : IDisposable
{
    protected readonly FileStream Data = data;

    public void Dispose() => Data.Dispose();
    public abstract bool IsCorrectFormat();

    public abstract string? RetrieveAlbumName();

    public abstract string? RetrieveAlbumYear();

    public abstract string? RetrieveAlbumArtist();

    public abstract string? RetrieveTrackTitle();

    public abstract string? RetrieveTrackArtist();

    public abstract string? RetrieveTrackNumber();

    public abstract IEnumerable<MetadataPicture> RetrievePictures();

    public abstract MetadataPicture? RetrievePicture(MediaType type);
}