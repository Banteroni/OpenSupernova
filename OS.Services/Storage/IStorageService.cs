namespace OS.Services.Storage;

public interface IStorageService
{
    public Task<bool> SaveFileAsync(Stream stream, string objectName);

    public Task<Stream?> GetFileStream(string objectName);

    public Task<bool> DeleteFileAsync(string objectName);

    public Task<bool> FileExistsAsync(string objectName);

    public Task<IEnumerable<string>> ListAllFilesAsync();
}