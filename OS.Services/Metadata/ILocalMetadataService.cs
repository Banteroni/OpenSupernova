namespace OS.Services.Metadata;

public interface ILocalMetadataService
{
    public string? RetrieveAlbumName(FileStream stream);

    public string? RetrieveAlbumYear(FileStream stream);

    public string? RetrieveAlbumArtist(FileStream stream);

    public string? RetrieveTrackTitle(FileStream stream);

    public string? RetrieveTrackArtist(FileStream stream);

    public string? RetrieveTrackNumber(FileStream stream);

    public MetadataPicture RetrievePicture(FileStream stream);
} 