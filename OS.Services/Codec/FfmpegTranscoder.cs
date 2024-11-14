using System.Diagnostics;
using Microsoft.Extensions.Logging;
using OS.Data.Options;
using OS.Services.Storage;

namespace OS.Services.Codec;

public class FfmpegTranscoder(
    ILogger<FfmpegTranscoder> logger,
    IStorageService storageService,
    ITempStorageService tempStorageService,
    TranscodeSettings transcodeSettings) : ITranscoder
{
    private readonly ILogger<FfmpegTranscoder> _logger = logger;
    private readonly IStorageService _storageService = storageService;
    private readonly ITempStorageService _tempStorageService = tempStorageService;
    private readonly TranscodeSettings _transcodeSettings = transcodeSettings;

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

    public async Task TranscodeAsync(string inputObject, string outputObject)
    {
        if (!await _tempStorageService.FileExistsAsync(inputObject))
        {
            _logger.LogError($"File {inputObject} does not exist");
            return;
        }

        var tempInputLocation = Path.Combine(_tempStorageService.GetPath(), inputObject);
        var tempOutputLocation = Path.Combine(_tempStorageService.GetPath(), outputObject);
        // check if file can be read and wrote
        var process = new Process();
        process.StartInfo.FileName = "ffmpeg";
        process.StartInfo.Arguments =
            $"-y -i {tempInputLocation} -f {_transcodeSettings.Format} -c:a {_transcodeSettings.Codec} -b:a {_transcodeSettings.BitrateKbps}k -ar {_transcodeSettings.SampleRate} -ac {_transcodeSettings.Channels} {tempOutputLocation}";
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        try
        {
            process.OutputDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    _logger.LogInformation(args.Data); // Log output if necessary
                }
            };

            process.ErrorDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    _logger.LogError(args.Data); // Log errors
                }
            };

            process.Start();

            // Start reading from the output and error streams asynchronously
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync();
            if (process.ExitCode != 0)
            {
                await _tempStorageService.DeleteFileAsync(tempOutputLocation);
                throw new Exception($"Transcoding failed, exit code: {process.ExitCode}");
            }

            // get byte array from file
            var bytes = await File.ReadAllBytesAsync(tempOutputLocation);

            var isFileStored = await _storageService.SaveFileAsync(bytes, outputObject);
            if (!isFileStored)
                throw new Exception($"Failed to store file {outputObject}");

            await _tempStorageService.DeleteFileAsync(tempOutputLocation);
            await _tempStorageService.DeleteFileAsync(inputObject);
            _logger.LogInformation($"Transcoding complete");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to encode file {inputObject}, {ex}");
        }
    }
}