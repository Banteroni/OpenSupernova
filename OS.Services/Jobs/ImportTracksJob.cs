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

    public static readonly JobKey Key = new JobKey(nameof(ImportTracksJob), "processing");


    public async Task Execute(IJobExecutionContext context)
    {
        var jobData = context.MergedJobDataMap;
        var fileNameFound = jobData.TryGetString("file", out var fileName);
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

        var filesToProcess = new List<string>();
        var isZipArchive = await _tempStorageService.IsFileZip(fileName);
        if (isZipArchive)
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
                var trackGuid = Guid.NewGuid();
                var fileStream = await _tempStorageService.GetFileStream(file);
                if (fileStream == null || fileStream.Length == 0)
                {
                    _logger.LogError($"Failed to get file {file}, skipping");
                    continue;
                }

                // Transcoding
                await _transcoder.TranscodeAsync(file, trackGuid + ".opus");

                // Metadata
                var trackFile = new FlacFile(fileStream);
                var title = trackFile.GetTrackTitle();
                var number = trackFile.GetTrackNumber();
                var trackArtists = trackFile.GetTrackArtists();
                var albumName = trackFile.GetAlbumName();
                var duration = trackFile.GetDuration();
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
                if (albumArtist == null)
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
                    var condition = ArtistConditionPresets.ArtistNameSearch(albumArtist);
                    var artistsInDb = (await _repository.GetListAsync<Artist>(condition)).ToList();
                    if (artistsInDb.Count == 0)
                    {
                        var newArtistInDb = await _repository.CreateAsync(new Artist()
                        {
                            Name = albumArtist
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
                    if (cover?.Data != null)
                    {
                        using (var stream = new MemoryStream(cover.Data))
                        {
                            if (stream != null)
                            {
                                await _storageService.SaveFileAsync(stream, $"{albumId}_cover");
                            }
                        }
                    }

                    album = await _repository.CreateAsync(new Album()
                    {
                        Id = albumId,
                        Name = albumName,
                        Year = albumYear ?? 0,
                        Artist = artist,
                        Genre = albumGenre
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
                List<Artist> artists = [];
                if (trackArtists.Count() == 0)
                {
                    artists.Add(artist);
                }
                else
                {
                    foreach (var trackArtist in trackArtists)
                    {
                        var artistSearch = ArtistConditionPresets.ArtistNameSearch(trackArtist);
                        var artistsInDb = (await _repository.GetListAsync<Artist>(artistSearch)).ToList();
                        if (artistsInDb.Count == 0)
                        {
                            var newArtist = await _repository.CreateAsync(new Artist()
                            {
                                Name = trackArtist
                            });
                            artists.Add(newArtist!);
                        }
                        else
                        {
                            artists.Add(artistsInDb.FirstOrDefault()!);
                        }
                    }
                }
                var track = await _repository.CreateAsync(new Track()
                {
                    Id = trackGuid,
                    Name = title,
                    Duration = duration,
                    Number = number ?? 0,
                    Album = album,
                    Artists = artists
                });
                _logger.LogInformation($"Track {track?.Name} added to the database");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to process file {file}, skipping");
            }
            await _tempStorageService.DeleteFileAsync(file);
        }
        await _tempStorageService.DeleteFileAsync(fileName);
    }
}