namespace OS.Services.Storage;

public class AzureStorageService : IStorageService
{
    public Task<bool> SaveFileAsync(byte[] stream, string objectName)
    {
        throw new NotImplementedException();
    }

    public Task<byte[]> GetFileAsync(string objectName)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteFileAsync(string objectName)
    {
        throw new NotImplementedException();
    }

    public Task<bool> FileExistsAsync(string objectName)
    {
        throw new NotImplementedException();
    }
}