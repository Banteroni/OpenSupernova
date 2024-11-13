using Microsoft.Extensions.Logging;
using Moq;
using OS.Data.Options;
using OS.Services.Codec;
using OS.Services.Jobs;
using OS.Services.Storage;
using Quartz;
using MockRepository = OS.Services.Repository.MockRepository;

namespace OS.Services.Tests.Jobs;

public class ImportTracksJobTest
{
    private ImportTracksJob _job;

    [SetUp]
    public async Task Setup()
    {
        var logger = new Mock<ILogger<ImportTracksJob>>();
        var storageLogger = new Mock<ILogger<LocalStorageService>>();
        var repository = new MockRepository();
        var storageService = new LocalStorageService(storageLogger.Object, "archive");
        var tempStorageService = new LocalStorageService(storageLogger.Object, "temp");
        TranscodeSettings transcodeSettings = new TranscodeSettings()
        {
            BitrateKbps = 128,
            SampleRate = 48000,
            Channels = 2,
            Codec = "libopus",
            Format = "opus"
        };
        var transcoderLogger = new Mock<ILogger<FfmpegTranscoder>>();
        var transcoder = new FfmpegTranscoder(transcoderLogger.Object, storageService, tempStorageService,
            transcodeSettings);

        var buffer = await File.ReadAllBytesAsync("dummy.flac");
        await tempStorageService.SaveFileAsync(buffer, "dummy.flac");

        _job = new ImportTracksJob(logger.Object, storageService, tempStorageService, transcoder, repository);
    }


    [Test]
    public async Task ExecutionComplete()
    {
        var jobData = new JobDataMap();
        jobData.Add("fileName", "dummy.flac");
        var context = new Mock<IJobExecutionContext>();
        context.Setup(x => x.JobDetail.JobDataMap).Returns(jobData);
        await _job.Execute(context.Object);
        Assert.Pass();
    }
}