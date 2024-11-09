namespace OS.Services.Storage;

public interface IStorageService
{
    public Task<bool> SaveFileAsync(Stream stream, string objectName);

    public Task<FileStream?> GetFileAsync(string objectName);

    public Task<bool> DeleteFileAsync(string objectName);

    public Task<bool> FileExistsAsync(string objectName);
}