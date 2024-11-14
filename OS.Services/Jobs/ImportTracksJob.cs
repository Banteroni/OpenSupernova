using Microsoft.Extensions.Logging;
using OS.Data.Files;
using OS.Data.Models;
using OS.Services.Codec;
using OS.Services.Repository;
using OS.Services.Storage;
using Quartz;

namespace OS.Services.Jobs;

public class ImportTracksJob(
    ILogger<ImportTracksJob> logger,
    IStorageService storageService,
    ITempStorageService tempStorageService,
    ITranscoder transcoderService,
    IRepository repository) : IJob
{
    private readonly ILogger<ImportTracksJob> _logger = logger;
    private readonly IStorageService _storageService = storageService;
    private readonly ITempStorageService _tempStorageService = tempStorageService;
    private readonly ITranscoder _transcoder = transcoderService;
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

        var temporaryFileBytes = await _tempStorageService.GetFileAsync(fileName);
        if (temporaryFileBytes.Length == 0)
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
                var isFileGuid = Guid.TryParse(file, out var fileGuid);
                if (!isFileGuid)
                { 
                    _logger.LogError("File name is not a valid GUID, this should not happen");
                    continue;
                }
                var fileBytes = await _tempStorageService.GetFileAsync(file);
                if (fileBytes.Length == 0)
                {
                    _logger.LogError($"Failed to get file {file}, skipping");
                    continue;
                }

                // Transcoding
                await _transcoder.TranscodeAsync(file, file + ".opus");

                // Metadata
                var trackFile = new FlacFile(fileBytes);
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

                // Database operations
                Artist artist;
                if (artistName == null)
                {
                    artist = await _repository.GetUnknownArtistAsync();
                }
                else
                {
                    var artistsInDb = (await _repository.GetArtistsAsync(artistName)).ToList();
                    if (artistsInDb.Count == 0)
                    {
                        artist = await _repository.CreateArtistAsync(new Artist()
                        {
                            Name = artistName
                        });
                    }
                    else
                    {
                        artist = artistsInDb.FirstOrDefault()!;
                    }
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
                        Id = albumId,
                        Name = albumName,
                        Year = albumYear,
                        Artist = artist,
                        Genre = albumGenre,
                        CoverPath = cover != null ? $"{albumId}_cover" : null
                    });
                }
                else
                {
                    album = albumsInDb.FirstOrDefault()!;
                }
                // Check if track already exists
                var tracksInDb = (await _repository.GetTracksAsync(title, number, album.Id)).ToList();
                if (tracksInDb.Count > 0)
                {
                    _logger.LogWarning($"Track {title} already exists in the database, skipping");
                    continue;
                }
                
                var track = await _repository.CreateTrackAsync(new Track()
                {
                    Id = fileGuid,
                    Name = title,
                    Number = number,
                    Album = album,
                    FileObject = file + ".opus"
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