using System.IO.Compression;
using Microsoft.Extensions.Logging;
using OS.Data.Files;
using OS.Data.Models;
using OS.Services.Storage;
using Quartz;
using System.Linq;

namespace OS.Services.Jobs;

public class ImportTracksJob(
    ILogger<BaseJob> logger,
    IStorageService storageService,
    ITempStorageService tempStorageService,
    IRepository.IRepository repository) : BaseJob(logger, storageService, tempStorageService, repository)
{
    public override string Name { get; init; } = nameof(ImportTracksJob);
    public override string Group { get; init; } = "ImportGroup";

    public override async Task<bool> ExecuteJob(IJobExecutionContext context)
    {
        var jobData = context.JobDetail.JobDataMap;
        var fileName = jobData.GetString("fileName");
        if (string.IsNullOrEmpty(fileName))
        {
            Logger.LogError("File path is empty");
            return false;
        }

        if (!await TempStorageService.FileExistsAsync(fileName))
        {
            Logger.LogError($"File {fileName} does not exist");
            return false;
        }

        var fileStream = await TempStorageService.GetFileAsync(fileName);
        if (fileStream == null)
        {
            Logger.LogError($"Failed to get file {fileName}");
            return false;
        }

        var filesToProcess = new List<string>();
        // If the file is zip archive, extract it
        if (Path.GetExtension(fileName) == ".zip")
        {
            var filesExtracted = await TempStorageService.ExtractZipAsync(fileName);
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
                await using var fileReader = new FileStream(file, FileMode.Open, FileAccess.Read);
                using var trackFile = new FlacFile(fileReader);
                var title = trackFile.GetTrackTitle();
                var number = trackFile.GetTrackNumber();
                var artistName = trackFile.GetTrackArtist();
                var albumName = trackFile.GetAlbumName();
                var albumYear = trackFile.GetAlbumYear();
                var albumGenre = trackFile.GetAlbumGenre();
                var albumArtist = trackFile.GetAlbumArtist();
                if (title == null)
                {
                    Logger.LogError($"Failed to get title from file {file}, skipping");
                    continue;
                }

                Artist artist;
                if (artistName == null)
                {
                    artist = await Repository.GetUnknownArtistAsync();
                }
                else
                {
                    var artistsInDb = (await Repository.GetArtistsAsync(artistName)).ToList();
                    artist = artistsInDb.FirstOrDefault() ?? await Repository.CreateArtistAsync(new Artist()
                    {
                        Name = artistName
                    });
                }

                Album album;
                if (albumName == null) continue;
                var albumsInDb = (await Repository.GetAlbumsAsync(albumName, albumYear, albumArtist)).ToList();
                if (albumsInDb.Count == 0)
                {
                    Guid albumId = Guid.NewGuid();
                    // Get picture from the file
                    using var cover = trackFile.GetPicture(MediaType.CoverFront);

                    if (cover != null)
                    {
                        await StorageService.SaveFileAsync(cover.Data, $"{albumId}_cover");
                    }


                    album = await Repository.CreateAlbumAsync(new Album()
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
                
                var track = await Repository.CreateTrackAsync(new Track()
                {
                    Name = title,
                    Number = number,
                    AlbumId = album.Id,
                });
                Logger.LogInformation($"Track {track.Name} added to the database");
                return true;
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Failed to process file {file}, skipping");
                return false;
            }
        }
        return true;
    }
}