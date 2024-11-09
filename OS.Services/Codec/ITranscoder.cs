namespace OS.Services.Codec;

public interface ITranscoder
{
    public Task<bool> AreDependenciesInstalled();
    public Task TranscodeAsync(string inputPath, string outputPath, string format, string codec, int bitrate, int sampleRate, string channels);
}