using Microsoft.EntityFrameworkCore;
using OS.Data.Context;
using OS.Data.Models;

namespace OS.Services.Repository;

public class SqlRepository(OsDbContext dbContext) : IRepository
{
    private readonly OsDbContext _dbContext = dbContext;

    public async Task<Album?> GetAlbumAsync(Guid id)
    {
        var album = await _dbContext.Albums.FindAsync(id);
        return album;
    }

    public async Task<IEnumerable<Album>> GetAlbumsAsync(string? title = null, int? year = null, string? artist = null)
    {
        var query = _dbContext.Albums.AsQueryable();
        if (title != null)
        {
            query = query.Where(a => a.Name.Contains(title));
        }

        if (year != null)
        {
            query = query.Where(a => a.Year == year);
        }

        if (artist != null)
        {
            query = query.Where(a => a.Artist != null &&  a.Artist.Name.Contains(artist));
        }

        var albums = await query.ToListAsync();
        return albums;
    }

    public async Task<Artist?> GetArtistAsync(Guid id)
    {
        var artist = await _dbContext.Artists.FindAsync(id);
        return artist;
    }

    public async Task<Artist> GetUnknownArtistAsync()
    {
        var artist = await _dbContext.Artists.FindAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"));
        if (artist == null)
        {
            throw new InvalidOperationException("Unknown artist not found, it should be seeded in the database");
        }

        return artist;
    }

    public async Task<IEnumerable<Artist>> GetArtistsAsync(string? name = null)
    {
        var query = _dbContext.Artists.AsQueryable();
        if (name != null)
        {
            query = query.Where(a => a.Name.Contains(name));
        }

        var artists = await query.ToListAsync();
        return artists;
    }

    public async Task<Track?> GetTrackAsync(Guid id)
    {
        var track = await _dbContext.Tracks.FindAsync(id);
        return track;
    }

    public async Task<IEnumerable<Track>> GetTracksAsync(string? title = null, int? number = null, Guid? albumId = null)
    {
        var query = _dbContext.Tracks.AsQueryable();
        if (title != null)
        {
            query = query.Where(t => t.Name.Contains(title));
        }
        if (number != null)
        {
            query = query.Where(t => t.Number == number);
        }
        if (albumId != null)
        {
            query = query.Where(t => t.Album != null && t.Album.Id == albumId);
        }
        var tracks = await query.ToListAsync();
        return tracks;
    }

    public async Task<Album> CreateAlbumAsync(Album album)
    {
        var entity = await _dbContext.Albums.AddAsync(album);
        await _dbContext.SaveChangesAsync();
        return entity.Entity;
    }

    public async Task<Artist> CreateArtistAsync(Artist artist)
    {
        var entity = await _dbContext.Artists.AddAsync(artist);
        await _dbContext.SaveChangesAsync();
        return entity.Entity;
    }

    public async Task<Track> CreateTrackAsync(Track track)
    {
        var entity = await _dbContext.Tracks.AddAsync(track);
        await _dbContext.SaveChangesAsync();
        return entity.Entity;
    }

    public async Task<Album?> UpdateAlbumAsync(Album album, Guid id)
    {
        var albumInDb = await _dbContext.Albums.FindAsync(id);
        if (albumInDb == null)
        {
            return null;
        }

        albumInDb.Name = album.Name;
        albumInDb.Genre = album.Genre;
        albumInDb.Year = album.Year;
        albumInDb.CoverPath = album.CoverPath;
        if (album.NavigationArtistId != null && album.NavigationArtistId != album.Artist?.Id)
        {
            var artist = await _dbContext.Artists.FindAsync(album.NavigationArtistId);
            if (artist == null)
            {
                throw new InvalidOperationException("Artist not found");
            }
            albumInDb.Artist = artist;
        }
        _dbContext.Albums.Update(albumInDb);

        await _dbContext.SaveChangesAsync();
        return album;
    }

    public async Task<Artist?> UpdateArtistAsync(Artist artist, Guid id)
    {
        var artistInDb = await _dbContext.Artists.FindAsync(id);
        if (artistInDb == null)
        {
            return null;
        }
        artistInDb.Name = artist.Name;
        artistInDb.Bio = artist.Bio;
        artistInDb.ImagePath = artist.ImagePath;
        _dbContext.Artists.Update(artistInDb);
        await _dbContext.SaveChangesAsync();
        return artist;
    }

    public async Task<Track?> UpdateTrackAsync(Track track, Guid id)
    {
        var trackInDb = await _dbContext.Tracks.FindAsync(id);
        if (trackInDb == null)
        {
            return null;
        }
        trackInDb.Name = track.Name;
        trackInDb.Duration = track.Duration;
        trackInDb.Number = track.Number;
        _dbContext.Tracks.Update(trackInDb);
        await _dbContext.SaveChangesAsync();
        return track;
    }

    public async Task<Album?> DeleteAlbumAsync(Guid id)
    {
        var album = await _dbContext.Albums.FindAsync(id);
        if (album == null)
        {
            return null;
        }

        _dbContext.Albums.Remove(album);
        await _dbContext.SaveChangesAsync();
        return album;
    }

    public async Task<Artist?> DeleteArtistAsync(Guid id)
    {
        var artist = await _dbContext.Artists.FindAsync(id);
        if (artist == null)
        {
            return null;
        }

        _dbContext.Artists.Remove(artist);
        await _dbContext.SaveChangesAsync();
        return artist;
    }

    public async Task<Track?> DeleteTrackAsync(Guid id)
    {
        var track = _dbContext.Tracks.Find(id);
        if (track == null)
        {
            return null;
        }

        _dbContext.Tracks.Remove(track);
        await _dbContext.SaveChangesAsync();
        return track;
    }
}