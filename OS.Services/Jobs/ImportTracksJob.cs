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

    public static readonly JobKey Key = new JobKey(nameof(ImportTracksJob), "processing");


    public async Task Execute(IJobExecutionContext context)
    {
        var savedStorageFiles = new List<string>();
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

                var storedStreamFile = new StoredEntity()
                {
                    Id = Guid.NewGuid(),
                    Type = StoredEntityType.Stream,
                    Mime = "audio/ogg"
                };

                // Transcoding
                await _transcoder.TranscodeAsync(file, storedStreamFile.Id.ToString());
                savedStorageFiles.Add(storedStreamFile.Id.ToString());

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
                Album? album;

                if (albumArtist == null)
                {
                    artist = (await _repository.GetAsync<Artist>(Guid.Parse("00000000-0000-0000-0000-000000000001")))!;
                    if (artist == null)
                    {
                        _logger.LogError("Unknown artist not found in the database, this should not happen");
                        continue;
                    }
                }
                else
                {

                    var artistsInDb = await _repository.FindAllAsync<Artist>(x => x.Name == albumArtist);
                    if (artistsInDb.Count() == 0)
                    {
                        var newArtistInDb = await _repository.CreateAsync(new Artist()
                        {
                            Name = albumArtist
                        }, false);
                        artist = newArtistInDb!;
                    }
                    else
                    {
                        artist = artistsInDb.FirstOrDefault()!;
                    }
                }

                if (albumName == null) continue;

                var albumsInDb = await _repository.FindAllAsync<Album>(x => x.Name == albumName && x.Artist.Name == albumArtist && x.Year == albumYear);

                if (albumsInDb.Count() == 0)
                {
                    album = await _repository.CreateAsync(new Album()
                    {
                        Name = albumName,
                        Year = albumYear ?? 0,
                        Artist = artist,
                        Genre = albumGenre
                    }, false);

                    var pictures = trackFile.GetPictures();
                    foreach (var picture in pictures)
                    {
                        var pictureGuid = Guid.NewGuid();
                        var pictureStream = new MemoryStream(picture.Data);
                        var isCover = picture.Type == MediaType.CoverFront;
                        var pictureStoredEntity = new StoredEntity()
                        {
                            Id = pictureGuid,
                            Type = isCover ? StoredEntityType.AlbumCover : StoredEntityType.AlbumHelper,
                            Mime = picture.MimeType,
                            Album = album
                        };
                        await _repository.CreateAsync(pictureStoredEntity, false);
                        await _storageService.SaveFileAsync(pictureStream, pictureStoredEntity.Id.ToString());
                        savedStorageFiles.Add(pictureStoredEntity.Id.ToString());
                    }
                }
                else
                {
                    album = albumsInDb.FirstOrDefault()!;
                }

                // Check if track already exists
                var tracksInDb = await _repository.FindAllAsync<Track>(x => x.Name == title && x.Number == number && x.Album != null && x.Album.Id == album.Id);
                if (tracksInDb.Count() > 0)
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
                        var artistsInDb = await _repository.FindAllAsync<Artist>(x => x.Name == trackArtist);
                        if (artistsInDb.Count() == 0)
                        {
                            var newArtist = await _repository.CreateAsync(new Artist()
                            {
                                Name = trackArtist
                            }, false);
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
                }, false);

                storedStreamFile.Track = track;
                if (Environment.GetEnvironmentVariable("StoreOrigin") == "1")
                {
                    var storedOrigin = new StoredEntity()
                    {
                        Id = Guid.NewGuid(),
                        Type = StoredEntityType.Origin,
                        Mime = "audio/ogg",
                        Track = track
                    };
                    await _repository.CreateAsync(storedOrigin, false);
                    await _storageService.SaveFileAsync(fileStream, storedOrigin.Id.ToString());
                    savedStorageFiles.Add(storedOrigin.Id.ToString());
                }

                await _repository.CreateAsync(storedStreamFile, false);

                await _repository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to process file {file}, skipping");
                savedStorageFiles.ForEach(async f => await _storageService.DeleteFileAsync(f));
            }
            finally
            {
                _logger.LogInformation($"File {file} added correctly");
            }
            await _tempStorageService.DeleteFileAsync(file);
        }
        await _tempStorageService.DeleteFileAsync(fileName);
    }
}