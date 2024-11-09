using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using OS.Services.Codec;
using OS.Services.Storage;

namespace OS.Services.Tests.Codec;

public class FfmpegTranscoderTest : IDisposable
{
    private FfmpegTranscoder _ffmpegTranscoder;
    private IStorageService _storageService;
    private string _localStorageTestingPath;

    [SetUp]
    public void Setup()
    {
        try
        {
            Directory.CreateDirectory("tmp");
        }
        catch (Exception ex)
        {
            throw new DirectoryNotFoundException("Couldn't create directory", ex);
        }


        _localStorageTestingPath = Path.Join(Directory.GetCurrentDirectory(), "tmp");
        var logger = new Mock<ILogger<FfmpegTranscoder>>();
        var storageLogger = new Mock<ILogger<LocalStorageService>>();
        var options = new LocalStorageOptions(_localStorageTestingPath);
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
    public async Task TransCode()
    {
        var flacFile = Path.Join(Directory.GetCurrentDirectory(), "dummy.flac");
        var outputFolder = Path.Join(_localStorageTestingPath, "EncodeFile.opus");
        await _ffmpegTranscoder.TranscodeAsync(flacFile, outputFolder, "opus", "libopus", 128, 48000, "2");
        var fileExists = await _storageService.FileExistsAsync(outputFolder);
        Assert.That(fileExists, Is.True);
    }

    [Test]
    public async Task TranscodeFileWithWrongPath()
    {
        var flacFile = Path.Join(Directory.GetCurrentDirectory(), "dummyBad.flac");
        var outputFolder = Path.Join(_localStorageTestingPath, "encode-wrong-path.opus");
        await _ffmpegTranscoder.TranscodeAsync(flacFile, outputFolder, "opus", "libopus", 128, 48000, "2");
        var fileExists = await _storageService.FileExistsAsync(outputFolder);
        Assert.That(fileExists, Is.False);
    }

    [Test]
    public async Task TranscodeWithBadSettings()
    {
        var flacFile = Path.Join(Directory.GetCurrentDirectory(), "dummy.flac");
        var outputFolder = Path.Join(_localStorageTestingPath, "encode-wrong-settings.opus");
        await _ffmpegTranscoder.TranscodeAsync(flacFile, outputFolder, "opus", "libopus", 960, 48000, "2");
        var fileExists = await _storageService.FileExistsAsync(outputFolder);
        Assert.That(fileExists, Is.False);
    }

    public void Dispose()
    {
        Directory.Delete("tmp", true);
    }
}