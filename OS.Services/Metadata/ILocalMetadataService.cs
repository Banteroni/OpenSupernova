namespace OS.Services.Metadata;

public interface ILocalMetadataService
{
    public Task<string> RetrieveAlbumName(FileStream stream);

    public Task<string> RetrieveArtistName(FileStream stream);

    public Task<string> RetrieveTrackTitle(FileStream stream);

    public Task<string> RetrieveTrackArtist(FileStream stream);

    public Task<int> RetrieveTrackNumber(FileStream stream);
}