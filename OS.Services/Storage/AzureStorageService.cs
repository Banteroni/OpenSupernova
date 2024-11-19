using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;

namespace OS.Services.Storage;

public class AzureStorageService : IStorageService
{
    private readonly BlobContainerClient _blobContainerClient;
    private readonly ILogger<AzureStorageService> _logger;

    public AzureStorageService(string connectionString, string container, ILogger<AzureStorageService> logger)
    {
        _logger = logger;
        var blobServiceClient = new BlobServiceClient(connectionString);
        _blobContainerClient = blobServiceClient.GetBlobContainerClient(container);
    }


    public async Task<bool> SaveFileAsync(byte[] fileBytes, string objectName)
    {
        await using var stream = new MemoryStream(fileBytes);
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

    public async Task<byte[]> GetFileAsync(string objectName)
    {
        try
        {
            var data = await _blobContainerClient.GetBlobClient(objectName).DownloadContentAsync();
            await using var ms = data.Value.Content.ToStream();
            var bytes = new byte[ms.Length];
            var bytesRead = await ms.ReadAsync(bytes, 0, (int)ms.Length);
            if (bytesRead != ms.Length)
            {
                _logger.LogError("Failed to read file from Azure Blob Storage");
                return Array.Empty<byte>();
            }

            return bytes;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to read file from Azure Blob Storage, {ex}");
            return Array.Empty<byte>();
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
}