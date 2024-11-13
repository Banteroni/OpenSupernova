using Microsoft.Extensions.Logging;
using OS.Data.Files;
using OS.Data.Models;
using OS.Services.Repository;
using OS.Services.Storage;
using Quartz;

namespace OS.Services.Jobs;

public class ImportTracksJob(
    ILogger<ImportTracksJob> logger,
    IStorageService storageService,
    ITempStorageService tempStorageService,
    IRepository repository) : IJob
{
    private readonly ILogger<ImportTracksJob> _logger = logger;
    private readonly IStorageService _storageService = storageService;
    private readonly ITempStorageService _tempStorageService = tempStorageService;
    private readonly IRepository _repository = repository;


    public async Task Execute(IJobExecutionContext context)
    {
        var jobData = context.JobDetail.JobDataMap;
        var fileName = jobData.GetString("fileName");
        if (string.IsNullOrEmpty(fileName))
        {
            _logger.LogError("File path is empty");
            return;
        }

        if (!await _tempStorageService.FileExistsAsync(fileName))
        {
            _logger.LogError($"File {fileName} does not exist");
            return;
        }

        var fileStream = await _tempStorageService.GetFileAsync(fileName);
        if (fileStream == null)
        {
            _logger.LogError($"Failed to get file {fileName}");
            return;
        }

        var filesToProcess = new List<string>();
        // If the file is zip archive, extract it
        if (Path.GetExtension(fileName) == ".zip")
        {
            var filesExtracted = await _tempStorageService.ExtractZipAsync(fileName);
            filesToProcess = filesExtracted.ToList();
        }
        else
        {
            filesToProcess.Add(fileName);
        }

        foreach (var file in filesToProcess)
        {
            try
            {
                await using var fileReader = await tempStorageService.GetFileAsync(file);
                if (fileReader == null)
                {
                    _logger.LogError($"Failed to get file {file}, skipping");
                    continue;
                }
                // get bytes 
                var trackBytes = new byte[fileReader.Length];
                await fileReader.ReadAsync(trackBytes.AsMemory(0, trackBytes.Length));
                var trackFile = new FlacFile(trackBytes);
                var title = trackFile.GetTrackTitle();
                var number = trackFile.GetTrackNumber();
                var artistName = trackFile.GetTrackArtist();
                var albumName = trackFile.GetAlbumName();
                var albumYear = trackFile.GetAlbumYear();
                var albumGenre = trackFile.GetAlbumGenre();
                var albumArtist = trackFile.GetAlbumArtist();
                if (title == null)
                {
                    _logger.LogError($"Failed to get title from file {file}, skipping");
                    continue;
                }

                Artist artist;
                if (artistName == null)
                {
                    artist = await _repository.GetUnknownArtistAsync();
                }
                else
                {
                    var artistsInDb = (await _repository.GetArtistsAsync(artistName)).ToList();
                    artist = artistsInDb.FirstOrDefault() ?? await _repository.CreateArtistAsync(new Artist()
                    {
                        Name = artistName
                    });
                }

                Album album;
                if (albumName == null) continue;
                var albumsInDb = (await _repository.GetAlbumsAsync(albumName, albumYear, albumArtist)).ToList();
                if (albumsInDb.Count == 0)
                {
                    Guid albumId = Guid.NewGuid();
                    // Get picture from the file
                    var cover = trackFile.GetPicture(MediaType.CoverFront);

                    if (cover != null)
                    {
                        await _storageService.SaveFileAsync(cover.Data, $"{albumId}_cover");
                    }


                    album = await _repository.CreateAlbumAsync(new Album()
                    {
                        Name = albumName,
                        Year = albumYear,
                        ArtistId = artist.Id,
                        Genre = albumGenre,
                        CoverPath = cover != null ? $"{albumId}_cover" : null
                    });
                }
                else
                {
                    album = albumsInDb.FirstOrDefault()!;
                }

                var track = await _repository.CreateTrackAsync(new Track()
                {
                    Name = title,
                    Number = number,
                    AlbumId = album.Id,
                });
                _logger.LogInformation($"Track {track.Name} added to the database");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to process file {file}, skipping");
            }
        }
    }
}