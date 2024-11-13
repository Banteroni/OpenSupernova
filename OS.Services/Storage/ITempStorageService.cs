namespace OS.Services.Storage;

public interface ITempStorageService : IStorageService
{
    public Task<IEnumerable<string>> ExtractZipAsync(string objectName);
    public string GetPath();
}