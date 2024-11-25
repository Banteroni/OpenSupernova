using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using System.IO.Compression;

namespace OS.Services.Storage;

public class AzureStorageService : IStorageService, ITempStorageService
{
    private readonly BlobContainerClient _blobContainerClient;
    private readonly ILogger<AzureStorageService> _logger;

    public AzureStorageService(string connectionString, string container, ILogger<AzureStorageService> logger)
    {
        _logger = logger;
        var blobServiceClient = new BlobServiceClient(connectionString);
        _blobContainerClient = blobServiceClient.GetBlobContainerClient(container);
    }


    public async Task<bool> SaveFileAsync(Stream stream, string objectName)
    {
        try
        {
            await _blobContainerClient.UploadBlobAsync(objectName, stream);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to save file to Azure Blob Storage, {ex}");
            return false;
        }
    }

    public async Task<Stream?> GetFileStream(string objectName)
    {
        try
        {
            var data = await _blobContainerClient.GetBlobClient(objectName).DownloadContentAsync();
            var ms = data.Value.Content.ToStream();
            return ms;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to read file from Azure Blob Storage, {ex}");
            return null;
        }
    }

    public async Task<bool> DeleteFileAsync(string objectName)
    {
        try
        {
            var blobClient = _blobContainerClient.GetBlobClient(objectName);
            await blobClient.DeleteAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to delete file from Azure Blob Storage, {ex}");
            return false;
        }
    }

    public async Task<bool> FileExistsAsync(string objectName)
    {
        var blobClient = _blobContainerClient.GetBlobClient(objectName);
        return await blobClient.ExistsAsync();
    }

    public async Task<IEnumerable<string>> ListAllFilesAsync()
    {
        var blobs = _blobContainerClient.GetBlobsAsync()
            .AsPages(default, int.MaxValue);

        var files = new List<string>();

        await foreach (var page in blobs)
        {
            foreach (var blob in page.Values)
            {
                files.Add(blob.Name);
            }
        }

        return files.AsEnumerable();
    }

    public async Task<IEnumerable<string>> ExtractZipAsync(string objectName)
    {
        // Load stream
        var stream = await GetFileStream(objectName);
        if (stream == null)
        {
            return new List<string>();
        }
        var zip = new List<string>();
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
        {
            foreach (var entry in archive.Entries)
            {
                var entryStream = entry.Open();
                var entryName = entry.FullName;
                await SaveFileAsync(entryStream, entryName);
                zip.Add(entryName);
            }
        }
        return zip.AsEnumerable();
    }

    public async Task<bool> IsFileZip(string objectName)
    {
        var blobClient = _blobContainerClient.GetBlobClient(objectName);
        using (var stream = blobClient.OpenRead())
        {
            var buffer = new byte[4];
            await stream.ReadAsync(buffer, 0, 4);
            return (buffer[0] == 0x50 && buffer[1] == 0x4B && buffer[2] == 0x03 && buffer[3] == 0x04);
        }
    }

    public string GetPath()
    {
        return string.Empty;
    }
}