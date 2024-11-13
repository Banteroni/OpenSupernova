using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using OS.Data.Options;
using OS.Services.Codec;
using OS.Services.Storage;

namespace OS.Services.Tests.Codec;

public class FfmpegTranscoderTest : IDisposable
{
    private FfmpegTranscoder _ffmpegTranscoder;
    private IStorageService _storageService;
    private ITempStorageService _tempStorageService;
    private string _lsTempPath;
    private string _lsPath;

    [SetUp]
    public void Setup()
    {
        try
        {
            Directory.CreateDirectory("tmp");
            Directory.CreateDirectory("archive");
        }
        catch (Exception ex)
        {
            throw new DirectoryNotFoundException("Couldn't create directory", ex);
        }


        _lsTempPath = Path.Join(Directory.GetCurrentDirectory(), "tmp");
        _lsPath = Path.Join(Directory.GetCurrentDirectory(), "archive");
        var logger = new Mock<ILogger<FfmpegTranscoder>>();
        var storageLogger = new Mock<ILogger<LocalStorageService>>();
        ITempStorageService tempStorageService = new LocalStorageService(storageLogger.Object, _lsTempPath);
        _tempStorageService = tempStorageService;
        IStorageService storageService = new LocalStorageService(storageLogger.Object, _lsPath);
        _storageService = storageService;

        TranscodeSettings transcodeSettings = new TranscodeSettings()
        {
            BitrateKbps = 128,
            SampleRate = 48000,
            Channels = 2,
            Codec = "libopus",
            Format = "opus"
        };
        File.Copy(Path.Join("dummy.flac"), Path.Join(_lsTempPath, "dummy.flac"), true);
        
        _ffmpegTranscoder =
            new FfmpegTranscoder(logger.Object, _storageService, _tempStorageService, transcodeSettings);
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
        var flacFile = Path.Join("dummy.flac");
        var outputFile = Path.Join("EncodeFile.opus");
        await _ffmpegTranscoder.TranscodeAsync(flacFile, outputFile);
        var fileExists = await _storageService.FileExistsAsync(outputFile);
        Assert.That(fileExists, Is.True);
    }

    [Test]
    public async Task TranscodeFileWithWrongPath()
    {
        var flacFile = Path.Join("dummyWrongPath.flac");
        var outputFile = Path.Join("EncodeFileWrongPath.opus");
        await _ffmpegTranscoder.TranscodeAsync(flacFile, outputFile);
        var fileExists = await _storageService.FileExistsAsync(outputFile);
        Assert.That(fileExists, Is.False);
    }

    [Test]
    public async Task TranscodeWithBadSettings()
    {
        var flacFile = Path.Join("dummyWrongPath.flac");
        var outputFile = Path.Join("EncodeFileWrongSettings.opus");
        await _ffmpegTranscoder.TranscodeAsync(flacFile, outputFile);
        var fileExists = await _storageService.FileExistsAsync(outputFile);
        Assert.That(fileExists, Is.False);
    }

    public void Dispose()
    {
        Directory.Delete("tmp", true);
        Directory.Delete("archive", true);
    }
}