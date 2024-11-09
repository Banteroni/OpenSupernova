using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using OS.Services.Codec;
using OS.Services.Storage;

namespace OS.Services.Tests.Codec;

public class FfmpegTranscoderTest
{
    private FfmpegTranscoder _ffmpegTranscoder;
    private IStorageService _storageService;
    private string _localStorageTestingPath;

    [SetUp]
    public void Setup()
    {
        var localStoragePath = Environment.GetEnvironmentVariable("LOCAL_STORAGE_PATH");
        if (localStoragePath == null)
        {
            throw new DirectoryNotFoundException();
        }

        if (!Path.Exists(localStoragePath))
        {
            try
            {
                Directory.CreateDirectory(localStoragePath);
            }
            catch (Exception ex)
            {
                throw new DirectoryNotFoundException("Couldn't create directory", ex);
            }
        }

        _localStorageTestingPath = localStoragePath;
        var logger = new Mock<ILogger<FfmpegTranscoder>>();
        var storageLogger = new Mock<ILogger<LocalStorageService>>();
        var options = new LocalStorageOptions(localStoragePath);
        var storageService = new LocalStorageService(storageLogger.Object, Options.Create(options));
        _storageService = storageService;
        _ffmpegTranscoder = new FfmpegTranscoder(logger.Object, _storageService);
    }

    [Test]
    public async Task IsFfmpegInstalled()
    {
        var result = await _ffmpegTranscoder.AreDependenciesInstalled();
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task EncodeFile()
    {
        var path = Environment.GetEnvironmentVariable("FLAC_TRACK_PATH");
        if (!Path.Exists(path))
        {
            throw new DirectoryNotFoundException();
        }

        var outputFolder = Path.Join(_localStorageTestingPath, "test.opus");
        await _ffmpegTranscoder.TranscodeAsync(path, outputFolder, "opus", "libopus", 128, 48000, "2");
        var fileExists = await _storageService.FileExistsAsync(outputFolder);
        Assert.That(fileExists, Is.True);
    }
}