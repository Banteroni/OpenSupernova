using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OS.Services.Storage;

public class LocalStorageService : IStorageService
{
    private readonly ILogger<LocalStorageService> _logger;
    private readonly string _path;

    public LocalStorageService(ILogger<LocalStorageService> logger, IOptions<LocalStorageOptions> options)
    {
        _logger = logger;
        var pathInOptions = options.Value.Path;
        // Check if directory exists
        if (!Directory.Exists(pathInOptions))
        {
            try
            {
                Directory.CreateDirectory(pathInOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to create directory {pathInOptions}, {ex}");
                throw new DirectoryNotFoundException();
            }
        }
        _path = pathInOptions;
    }

    public async Task<bool> SaveFileAsync(Stream stream, string path)
    {
        var fullPath = Path.Combine(_path, path);

        try
        {
            await using var fileStream = File.Create(fullPath);
            await stream.CopyToAsync(fileStream);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to save file to {fullPath}, {ex}");
            return false;
        }
    }

    public Task<FileStream?> GetFileAsync(string path)
    {
        var fullPath = Path.Combine(_path, path);

        try
        {
            var fileStream = File.OpenRead(path);
            return Task.FromResult<FileStream?>(fileStream);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to get file from {fullPath}, {ex}");
            return Task.FromResult<FileStream?>(null);
        }
    }

    public Task<bool> DeleteFileAsync(string path)
    {
        var fullPath = Path.Combine(_path, path);

        try
        {
            File.Delete(fullPath);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to delete file from {fullPath}, {ex}");
            return Task.FromResult(false);
        }
    }

    public Task<bool> FileExistsAsync(string path)
    {
        var fullPath = Path.Combine(_path, path);
        return Task.FromResult(File.Exists(fullPath));
    }
}