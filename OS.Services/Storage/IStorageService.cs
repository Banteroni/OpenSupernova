namespace OS.Services.Storage;

public interface IStorageService
{
    public Task<bool> SaveFileAsync(byte[] stream, string objectName);

    public Task<byte[]> GetFileAsync(string objectName);

    public Task<bool> DeleteFileAsync(string objectName);

    public Task<bool> FileExistsAsync(string objectName);

    public Task<IEnumerable<string>> ListAllFilesAsync();
}