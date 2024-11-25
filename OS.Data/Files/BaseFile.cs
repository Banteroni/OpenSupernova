namespace OS.Data.Files;

public abstract class BaseFile(Stream data)
{
    protected readonly Stream Data = data;

    public abstract bool IsCorrectFormat();

    public abstract string? GetAlbumGenre();

    public abstract string? GetAlbumName();

    public abstract int? GetAlbumYear();

    public abstract string? GetAlbumArtist();

    public abstract string? GetTrackTitle();

    public abstract int GetDuration();

    public abstract IEnumerable<string> GetTrackArtists();

    public abstract string? GetTrackPerformer();

    public abstract int? GetTrackNumber();

    public abstract IEnumerable<MetadataPicture> GetPictures();

    public abstract MetadataPicture? GetPicture(MediaType type);
}