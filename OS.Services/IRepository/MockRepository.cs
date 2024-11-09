using OS.Data.Models;

namespace OS.Services.IRepository;

public class MockRepository : IRepository
{
    private List<Artist> _artists =
    [
        new Artist()
        {
            Id = Guid.Parse("246edfd0-1938-4fa8-8619-7597c9f03013"),
            Name = "Oasis",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        },
        new Artist()
        {
            Id = Guid.Parse("a6175e05-58ca-4b72-8139-87cbc052c88a"),
            Name = "Blur",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        },
        new Artist()
        {
            Id = Guid.Parse("r3455e05-58ca-4b72-8139-12cva052c88a"),
            Name = "Unknown",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        },
    ];

    private List<Album> _albums =
    [
        new Album()
        {
            Id = Guid.Parse("d0b03b74-319a-4b59-9d78-01518e5d0219"),
            Name = "Be Here Now",
            Genre = "Rock",
            Year = 1997,
            ArtistId = Guid.Parse("246edfd0-1938-4fa8-8619-7597c9f03013"),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        },
        new Album()
        {
            Id = Guid.Parse("53b89903-15c7-4c74-a33e-e1383caa1ac1"),
            Name = "Definitely Maybe",
            Genre = "Indie Rock",
            Year = 1994,
            ArtistId = Guid.Parse("246edfd0-1938-4fa8-8619-7597c9f03013"),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        },
        new Album()
        {
            Id = Guid.Parse("1375e64e-f0f5-45ff-923f-fc5515107a73"),
            Name = "The Great Escape",
            Genre = "Britpop",
            Year = 1995,
            ArtistId = Guid.Parse("a6175e05-58ca-4b72-8139-87cbc052c88a"),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        }
    ];

    private List<Track> _tracks =
    [
        new Track()
        {
            Id = Guid.Parse("d0b03b74-319a-4b59-9d78-01518e5d0219"),
            Name = "D'You Know What I Mean?",
            // in seconds
            Duration = 442,
            Number = 1,
            AlbumId = Guid.Parse("d0b03b74-319a-4b59-9d78-01518e5d0219"),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        },
        new Track()
        {
            Id = Guid.Parse("53b89903-15c7-4c74-a33e-e1383caa1ac1"),
            Name = "Roll With It",
            Duration = 235,
            Number = 2,
            AlbumId = Guid.Parse("d0b03b74-319a-4b59-9d78-01518e5d0219"),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        },
        new Track()
        {
            Id = Guid.Parse("1375e64e-f0f5-45ff-923f-fc5515107a73"),
            Name = "Country House",
            Duration = 257,
            Number = 1,
            AlbumId = Guid.Parse("1375e64e-f0f5-45ff-923f-fc5515107a73"),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        },
        new Track()
        {
            AlbumId = Guid.Parse("53b89903-15c7-4c74-a33e-e1383caa1ac1"),
            CreatedAt = DateTime.Now,
            Duration = 287,
            Id = Guid.Parse("1375e64e-f0f5-45ff-923f-fc5515107a73"),
            Name = "Rock 'n' Roll Star",
            Number = 1,
            UpdatedAt = DateTime.Now
        }
    ];


    public Task<Album?> GetAlbumAsync(Guid id)
    {
        return Task.FromResult(_albums.FirstOrDefault(a => a.Id == id));
    }

    public Task<IEnumerable<Album>> GetAlbumsAsync(string? title = null, int? year = null, string? artist = null)
    {
        return Task.FromResult(_albums.Where(a => title != null && a.Name == title  || year != null && a.Year == year || artist != null && a.Artist.Name == artist).AsEnumerable());
    }

    public Task<Artist?> GetArtistAsync(Guid id)
    {
        return Task.FromResult(_artists.FirstOrDefault(a => a.Id == id));
    }

    public Task<Artist> GetUnknownArtistAsync()
    {
        return Task.FromResult(_artists.FirstOrDefault(a => a.Name == "Unknown"))!;
    }

    public Task<IEnumerable<Artist>> GetArtistsAsync(string? name = null)
    {
        return Task.FromResult(_artists.Where(a => name != null && a.Name == name).AsEnumerable());
    }

    public Task<Track?> GetTrackAsync(Guid id)
    {
        return Task.FromResult(_tracks.FirstOrDefault(t => t.Id == id));
    }

    public Task<IEnumerable<Track>> GetTracksAsync()
    {
        return Task.FromResult(_tracks.AsEnumerable());
    }

    public Task<Album> CreateAlbumAsync(Album album)
    {
        var newAlbum = new Album()
        {
            Id = Guid.NewGuid(),
            Name = album.Name,
            Genre = album.Genre,
            Year = album.Year,
            ArtistId = album.ArtistId,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        _albums.Add(newAlbum);
        return Task.FromResult(newAlbum);
    }

    public Task<Artist> CreateArtistAsync(Artist artist)
    {
        var newArtist = new Artist()
        {
            Id = Guid.NewGuid(),
            Name = artist.Name,
            Bio = artist.Bio,
            ImagePath = artist.ImagePath,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        _artists.Add(newArtist);
        return Task.FromResult(newArtist);
    }

    public Task<Track> CreateTrackAsync(Track track)
    {
        var newTrack = new Track()
        {
            Id = Guid.NewGuid(),
            Name = track.Name,
            Duration = track.Duration,
            Number = track.Number,
            AlbumId = track.AlbumId,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        _tracks.Add(newTrack);
        return Task.FromResult(newTrack);
    }

    public Task<Album> UpdateAlbumAsync(Album album)
    {
        var existingAlbum = _albums.FirstOrDefault(a => a.Id == album.Id);
        if (existingAlbum == null)
        {
            throw new Exception("Album not found");
        }

        existingAlbum.Name = album.Name;
        existingAlbum.Genre = album.Genre;
        existingAlbum.Year = album.Year;
        existingAlbum.ArtistId = album.ArtistId;
        existingAlbum.UpdatedAt = DateTime.Now;

        return Task.FromResult(existingAlbum);
    }

    public Task<Artist?> UpdateArtistAsync(Artist artist)
    {
        var existingArtist = _artists.FirstOrDefault(a => a.Id == artist.Id);
        if (existingArtist == null)
        {
            return null;
        }

        existingArtist.Name = artist.Name;
        existingArtist.Bio = artist.Bio;
        existingArtist.ImagePath = artist.ImagePath;
        existingArtist.UpdatedAt = DateTime.Now;

        return Task.FromResult(existingArtist);
    }

    public Task<Track?> UpdateTrackAsync(Track track)
    {
        var existingTrack = _tracks.FirstOrDefault(t => t.Id == track.Id);
        if (existingTrack == null)
        {
            return null;
        }

        existingTrack.Name = track.Name;
        existingTrack.Duration = track.Duration;
        existingTrack.Number = track.Number;
        existingTrack.AlbumId = track.AlbumId;
        existingTrack.UpdatedAt = DateTime.Now;

        return Task.FromResult(existingTrack);
    }

    public Task<Album?> DeleteAlbumAsync(Guid id)
    {
        var album = _albums.FirstOrDefault(a => a.Id == id);
        if (album == null)
        {
            return null;
        }

        _albums.Remove(album);
        return Task.FromResult(album);
    }

    public Task<Artist?> DeleteArtistAsync(Guid id)
    {
        var artist = _artists.FirstOrDefault(a => a.Id == id);
        if (artist == null)
        {
            return null;
        }

        _artists.Remove(artist);
        return Task.FromResult(artist);
    }

    public Task<Track?> DeleteTrackAsync(Guid id)
    {
        var track = _tracks.FirstOrDefault(t => t.Id == id);
        if (track == null)
        {
            return null;
        }

        _tracks.Remove(track);
        return Task.FromResult(track);
    }
}