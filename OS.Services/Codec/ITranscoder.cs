namespace OS.Services.Codec;

public interface ITranscoder
{
    public Task<bool> AreDependenciesInstalled();
    public Task TranscodeAsync(string inputObject, string outputObject);
}