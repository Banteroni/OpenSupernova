using System.Diagnostics;
using Microsoft.Extensions.Logging;
using OS.Services.Storage;

namespace OS.Services.Codec;

public class FfmpegTranscoder(ILogger<FfmpegTranscoder> logger, IStorageService storageService) : ITranscoder
{
    private readonly ILogger<FfmpegTranscoder> _logger = logger;
    private readonly IStorageService _storageService = storageService;

    public async Task<bool> AreDependenciesInstalled()
    {
        var process = new Process();
        process.StartInfo.FileName = "ffmpeg";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false;
        try
        {
            process.Start();
            var response = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();
            return true;
        }
        catch (Exception)
        {
            _logger.LogError("FFmpeg is not installed");
            return false;
        }
    }

    public async Task TranscodeAsync(string inputPath, string outputPath, string format, string codec, int bitrate,
        int sampleRate,
        string channels)
    {
        if (!await _storageService.FileExistsAsync(inputPath))
        {
            _logger.LogError($"File {inputPath} does not exist");
            return;
        }
        
        var process = new Process();
        process.StartInfo.FileName = "ffmpeg";
        process.StartInfo.Arguments =
            $"-y -i {inputPath} -f {format} -c:a {codec} -b:a {bitrate}k -ar {sampleRate} -ac {channels} {outputPath}";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false;
        try
        {
            process.Start();
            var response = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();
            if (!await _storageService.FileExistsAsync(outputPath))
                throw new FileNotFoundException(
                    $"Transcoding was completed, however the file {outputPath} was not found");

            if (process.ExitCode != 0)
            {
                await _storageService.DeleteFileAsync(outputPath);
                throw new Exception($"Transcoding failed, exit code: {process.ExitCode}");
            }
            
            _logger.LogInformation($"Transcoding complete, output: {response}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to encode file {inputPath}, {ex}");
        }
    }
}