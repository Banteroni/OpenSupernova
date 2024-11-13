using OS.Data.Models;

namespace OS.Services.Repository;

public class SqlRepository : IRepository
{
    public Task<Album?> GetAlbumAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Album>> GetAlbumsAsync(string? title = null, int? year = null, string? artist = null)
    {
        throw new NotImplementedException();
    }

    public Task<Artist?> GetArtistAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Artist> GetUnknownArtistAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Artist>> GetArtistsAsync(string? name = null)
    {
        throw new NotImplementedException();
    }

    public Task<Track?> GetTrackAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Track>> GetTracksAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Album> CreateAlbumAsync(Album album)
    {
        throw new NotImplementedException();
    }

    public Task<Artist> CreateArtistAsync(Artist artist)
    {
        throw new NotImplementedException();
    }

    public Task<Track> CreateTrackAsync(Track track)
    {
        throw new NotImplementedException();
    }

    public Task<Album?> UpdateAlbumAsync(Album album)
    {
        throw new NotImplementedException();
    }

    public Task<Artist?> UpdateArtistAsync(Artist artist)
    {
        throw new NotImplementedException();
    }

    public Task<Track?> UpdateTrackAsync(Track track)
    {
        throw new NotImplementedException();
    }

    public Task<Album?> DeleteAlbumAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Artist?> DeleteArtistAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Track?> DeleteTrackAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}