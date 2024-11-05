namespace OS.Services.Metadata;

public class Mp3LocalMetadataService : ILocalMetadataService
{
    public Task<string> RetrieveAlbumName(FileStream stream)
    {
        throw new NotImplementedException();
    }

    public Task<string> RetrieveArtistName(FileStream stream)
    {
        throw new NotImplementedException();
    }

    public Task<string> RetrieveTrackTitle(FileStream stream)
    {
        throw new NotImplementedException();
    }

    public Task<string> RetrieveTrackArtist(FileStream stream)
    {
        throw new NotImplementedException();
    }

    public Task<int> RetrieveTrackNumber(FileStream stream)
    {
        throw new NotImplementedException();
    }
}