using OS.Data.Models;

namespace OS.Services.Repository;

public class MockRepository : IRepository
{
    private static List<Artist> _artists =
    [
        new Artist()
        {
            Id = Guid.Parse("58a37044-9560-4c7e-8d7f-a904a635b83f"),
            Name = "Oasis",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        },
        new Artist()
        {
            Id = Guid.Parse("78084a3e-ab23-464d-9cb4-72080f57a22d"),
            Name = "Blur",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        },
        new Artist()
        {
            Id = Guid.Parse("5e2c6162-fbb9-472c-8c00-d09eb45c041f"),
            Name = "Unknown",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        },
    ];

    private static List<Album> _albums =
    [
        new Album()
        {
            Id = Guid.Parse("d0b03b74-319a-4b59-9d78-01518e5d0219"),
            Name = "Be Here Now",
            Genre = "Rock",
            Year = 1997,
            Artist = _artists[0],
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        },
        new Album()
        {
            Id = Guid.Parse("53b89903-15c7-4c74-a33e-e1383caa1ac1"),
            Name = "Definitely Maybe",
            Genre = "Indie Rock",
            Year = 1994,
            Artist = _artists[0],
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        },
        new Album()
        {
            Id = Guid.Parse("1375e64e-f0f5-45ff-923f-fc5515107a73"),
            Name = "The Great Escape",
            Genre = "Britpop",
            Year = 1995,
            Artist = _artists[1],
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
            Album = _albums[0],
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        },
        new Track()
        {
            Id = Guid.Parse("53b89903-15c7-4c74-a33e-e1383caa1ac1"),
            Name = "Roll With It",
            Duration = 235,
            Number = 2,
            Album = _albums[1],
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        },
        new Track()
        {
            Id = Guid.Parse("1375e64e-f0f5-45ff-923f-fc5515107a73"),
            Name = "Country House",
            Duration = 257,
            Number = 1,
            Album = _albums[2],
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        },
        new Track()
        {
            Album = _albums[1],
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
        var queriedAlbums = _albums
            .Where(a =>
                (title == null || a.Name == title) &&
                (year == null || a.Year == year) &&
                (artist == null || a.Artist.Name == artist)).ToList();

        for (var i = 0; i < queriedAlbums.Count; i++)
        {
            var album = queriedAlbums[i];
            album.Artist = _artists.First(a => a.Id == album.Artist?.Id);
            album.Tracks = _tracks.Where(t => t.Album?.Id == album.Id).ToList();
        }

        return Task.FromResult(queriedAlbums.AsEnumerable());
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

    public Task<IEnumerable<Track>> GetTracksAsync(string? title = null, int? number = null, Guid? albumId = null)
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
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        _tracks.Add(newTrack);
        return Task.FromResult(newTrack);
    }

    public Task<Album> UpdateAlbumAsync(Album album, Guid id)
    {
        var existingAlbum = _albums.FirstOrDefault(a => a.Id == id);
        if (existingAlbum == null)
        {
            throw new Exception("Album not found");
        }

        existingAlbum.Name = album.Name;
        existingAlbum.Genre = album.Genre;
        existingAlbum.Year = album.Year;
        existingAlbum.CoverPath = album.CoverPath;
        if (album.NavigationArtistId != null && album.Artist == null)
        {
            existingAlbum.Artist = _artists.FirstOrDefault(a => a.Id == album.NavigationArtistId);
        }
        else
        {
            existingAlbum.Artist = album.Artist;
        }

        existingAlbum.UpdatedAt = DateTime.Now;

        return Task.FromResult(existingAlbum);
    }

    public Task<Artist?> UpdateArtistAsync(Artist artist, Guid id)
    {
        var existingArtist = _artists.FirstOrDefault(a => a.Id == id);
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

    public Task<Track?> UpdateTrackAsync(Track track, Guid id)
    {
        var existingTrack = _tracks.FirstOrDefault(t => t.Id == id);
        if (existingTrack == null)
        {
            return null;
        }

        existingTrack.Name = track.Name;
        existingTrack.Duration = track.Duration;
        existingTrack.Number = track.Number;
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