﻿using OS.Data.Models;

namespace OS.Services.Repository;

public interface IRepository
{
    public Task<Album?> GetAlbumAsync(Guid id);
    
    public Task<IEnumerable<Album>> GetAlbumsAsync(string? title = null, int? year = null, string? artist = null);

    public Task<Artist?> GetArtistAsync(Guid id);
    
    public Task<Artist> GetUnknownArtistAsync();

    public Task<IEnumerable<Artist>> GetArtistsAsync(string? name = null);

    public Task<Track?> GetTrackAsync(Guid id);

    public Task<IEnumerable<Track>> GetTracksAsync(string? title = null, int? number = null, Guid? albumId = null);

    public Task<Album> CreateAlbumAsync(Album album);

    public Task<Artist> CreateArtistAsync(Artist artist);

    public Task<Track> CreateTrackAsync(Track track);

    public Task<Album?> UpdateAlbumAsync(Album album, Guid id);

    public Task<Artist?> UpdateArtistAsync(Artist artist, Guid id);

    public Task<Track?> UpdateTrackAsync(Track track, Guid id);

    public Task<Album?> DeleteAlbumAsync(Guid id);

    public Task<Artist?> DeleteArtistAsync(Guid id);

    public Task<Track?> DeleteTrackAsync(Guid id);
}