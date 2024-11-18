using Microsoft.Extensions.Logging;
using OS.Data.Files;
using OS.Data.Models;
using OS.Data.Repository.ConditionPresets;
using OS.Data.Repository.Conditions;
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
        var fileNameFound = jobData.TryGetString("fileName", out var fileName);
        if (!fileNameFound)
        {
            _logger.LogError("File path not found in the job data");
            return;
        }
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
                    var unknownArtist = ArtistConditionPresets.UnknownArtist();
                    var isUnknownArtistParsed = Guid.TryParse(unknownArtist.Value.ToString(), out var unknownArtistId);
                    if (!isUnknownArtistParsed)
                    {
                        _logger.LogError("Failed to parse unknown artist ID, this should not happen");
                        continue;
                    }

                    artist = (await _repository.GetAsync<Artist>(unknownArtistId))!;
                    if (artist == null)
                    {
                        throw new Exception("Unknown artist not found in the database, this should not happen");
                    }
                }
                else
                {
                    var condition = ArtistConditionPresets.ArtistNameSearch(artistName);
                    var artistsInDb = (await _repository.GetListAsync<Artist>(condition)).ToList();
                    if (artistsInDb.Count == 0)
                    {
                        var newArtistInDb = await _repository.CreateAsync(new Artist()
                        {
                            Name = artistName
                        });
                        artist = newArtistInDb!;
                    }
                    else
                    {
                        artist = artistsInDb.FirstOrDefault()!;
                    }
                }

                Album? album;
                if (albumName == null) continue;
                var albumSearch = AlbumConditionPresets.AlbumSearch(albumName, albumArtist, albumYear);
                var albumsInDb = (await _repository.GetListAsync<Album>(albumSearch)).ToList();
                if (albumsInDb.Count == 0)
                {
                    var albumId = Guid.NewGuid();
                    // Get picture from the file
                    var cover = trackFile.GetPicture(MediaType.CoverFront);

                    if (cover != null)
                    {
                        await _storageService.SaveFileAsync(cover.Data, $"{albumId}_cover");
                    }


                    album = await _repository.CreateAsync(new Album()
                    {
                        Id = albumId,
                        Name = albumName,
                        Year = albumYear ?? 0,
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
                var compositeTrackCondition = new CompositeCondition(LogicalSwitch.And);
                compositeTrackCondition.AddCondition(new SimpleCondition("Name", Operator.Contains, title));
                if (number != null)
                {
                    compositeTrackCondition.AddCondition(new SimpleCondition("Number", Operator.Equal, (int)number));
                }
                compositeTrackCondition.AddCondition(new SimpleCondition("Id", Operator.Equal,
                    album.Id, nameof(Album)));
                var tracksInDb = (await _repository.GetListAsync<Track>(compositeTrackCondition)).ToList();
                if (tracksInDb.Count > 0)
                {
                    _logger.LogWarning($"Track {title} already exists in the database, skipping");
                    continue;
                }

                var track = await _repository.CreateAsync(new Track()
                {
                    Id = fileGuid,
                    Name = title,
                    Number = number ?? 0,
                    Album = album,
                    FileObject = file + ".opus"
                });
                _logger.LogInformation($"Track {track?.Name} added to the database");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to process file {file}, skipping");
                await _tempStorageService.DeleteFileAsync(file);
            }

        }
        await _tempStorageService.DeleteFileAsync(fileName);
    }
}