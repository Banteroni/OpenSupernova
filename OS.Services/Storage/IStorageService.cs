namespace OS.Services.Storage;

public interface IStorageService
{
    public Task<bool> SaveFileAsync(Stream stream, string path);

    public Task<FileStream?> GetFileAsync(string path);

    public Task<bool> DeleteFileAsync(string path);

    public Task<bool> FileExistsAsync(string path);
}