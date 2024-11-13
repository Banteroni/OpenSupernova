namespace OS.Data.Files;

public abstract class BaseFile(byte[] data)
{
    protected readonly byte[] Data = data;
    
    public abstract bool IsCorrectFormat();

    public abstract string? GetAlbumGenre();
    
    public abstract string? GetAlbumName();

    public abstract int? GetAlbumYear();

    public abstract string? GetAlbumArtist();

    public abstract string? GetTrackTitle();

    public abstract string? GetTrackArtist();
    
    public abstract string? GetTrackPerformer();

    public abstract int? GetTrackNumber();

    public abstract IEnumerable<MetadataPicture> GetPictures();

    public abstract MetadataPicture? GetPicture(MediaType type);
}