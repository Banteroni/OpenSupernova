using OS.Data.Models;
using OS.Services.Repository;

namespace OS.Services.Tests.Repository;

public static class RepositoryUtils
{
    public static void PopulateWithBasicData(MockRepository repository)
    {
        var unknownArtist = new Artist()
            { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Name = "Unknown Artist" };
        repository.Artists.Add(unknownArtist);
        for (int i = 1; i <= 30; i++)
        {
            // Pick guids through for loop
            Guid guid = Guid.Parse($"00000000-0000-0000-0000-0000000000{i:D2}");
            var artist = new Artist() { Id = guid, Name = $"Artist {i}" };
            repository.Artists.Add(artist);
        }
        
        // Add albums
        for (int i = 1; i <= 30; i++)
        {
            Guid guid = Guid.Parse($"{i:D2}000000-0000-0000-0000-000000000000");
            var album = new Album()
            {
                Id = guid,
                Name = $"Album {i}",
                Year = 1980 + i,
                Artist = repository.Artists[0]
            };
            repository.Albums.Add(album);
        }
        
        // Add tracks, for each album add 10 tracks
        
        for (int i = 1; i <= 30; i++)
        {
            var album = repository.Albums[i - 1];
            for (int j = 1; j <= 10; j++)
            {
                Guid guid = Guid.Parse($"{i:D2}{j:D2}0000-0000-0000-0000-000000000000");
                var track = new Track()
                {
                    Id = guid,
                    Name = $"Track {j}",
                    Album = album
                };
                repository.Tracks.Add(track);
            }
        }
    }
}