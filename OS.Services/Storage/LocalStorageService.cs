﻿using System.IO.Compression;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OS.Services.Storage;

public class LocalStorageService : IStorageService, ITempStorageService
{
    private readonly ILogger<LocalStorageService> _logger;
    private readonly string _path;

    public LocalStorageService(ILogger<LocalStorageService> logger, string path)
    {
        _logger = logger;
        path = Path.Combine(Directory.GetCurrentDirectory(), path);
        // Check if directory exists
        if (!Directory.Exists(path))
        {
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to create directory {path}, {ex}");
                throw new DirectoryNotFoundException();
            }
        }

        _path = path;
    }

    public async Task<bool> SaveFileAsync(byte[] bytes, string objectName)
    {
        var fullPath = Path.Combine(_path, objectName);

        try
        {
            await File.WriteAllBytesAsync(fullPath, bytes);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to save file to {fullPath}, {ex}");
            return false;
        }
    }

    public Task<FileStream?> GetFileAsync(string objectName)
    {
        var fullPath = Path.Combine(_path, objectName);

        try
        {
            var fileStream = File.OpenRead(fullPath);
            return Task.FromResult<FileStream?>(fileStream);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to get file from {fullPath}, {ex}");
            return Task.FromResult<FileStream?>(null);
        }
    }

    public Task<bool> DeleteFileAsync(string objectName)
    {
        var fullPath = Path.Combine(_path, objectName);

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

    public Task<bool> FileExistsAsync(string objectName)
    {
        var fullPath = Path.Combine(_path, objectName);
        return Task.FromResult(File.Exists(fullPath));
    }

    public async Task<IEnumerable<string>> ExtractZipAsync(string objectName)
    {
        List<string> extractedFiles = [];
        var zipPath = Path.Combine(_path, objectName);
        try
        {
            using var archive = ZipFile.OpenRead(zipPath);
            foreach (var entry in archive.Entries)
            {
                var entryName = Guid.NewGuid().ToString();
                var entryPath = Path.Combine(_path, entryName);
                try
                {
                    entry.ExtractToFile(entryPath);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to extract file {entry.FullName}, {ex}");
                }
                finally
                {
                    extractedFiles.Add(entryName);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to extract zip archive {zipPath}, {ex}");
        }
        finally
        {
            File.Delete(zipPath);
        }

        return extractedFiles.AsEnumerable();
    }

    public string GetPath()
    {
        return _path;
    }
}