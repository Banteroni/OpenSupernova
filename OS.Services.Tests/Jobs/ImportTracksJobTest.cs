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
    private Mock<ILogger<ImportTracksJob>> _jobLogger;
    private Mock<ILogger<LocalStorageService>> _storageLogger;

    [SetUp]
    public async Task Setup()
    {
        _jobLogger = new Mock<ILogger<ImportTracksJob>>();
        _storageLogger = new Mock<ILogger<LocalStorageService>>();
        var repository = new MockRepository();
        var storageService = new LocalStorageService(_storageLogger.Object, "archive");
        var tempStorageService = new LocalStorageService(_storageLogger.Object, "temp");
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

        _job = new ImportTracksJob(_jobLogger.Object, storageService, tempStorageService, transcoder, repository);
    }


    [Test]
    public async Task ExecutionComplete()
    {
        // Arrange
        var jobData = new JobDataMap();
        jobData.Add("fileName", "dummy.flac");
        var context = new Mock<IJobExecutionContext>();
        context.Setup(x => x.JobDetail.JobDataMap).Returns(jobData);

        //Act
        await _job.Execute(context.Object);

        //Assert
        Assert.Pass();
    }

    [Test]
    public async Task FileDoesNotExist()
    {
        // Arrange
        var jobData = new JobDataMap();
        jobData.Add("fileName", "empty.flac");
        var context = new Mock<IJobExecutionContext>();
        context.Setup(x => x.JobDetail.JobDataMap).Returns(jobData);

        //Act
        await _job.Execute(context.Object);

        //Assert
        _jobLogger.Verify(
    m => m.Log(
        LogLevel.Error,
        It.IsAny<EventId>(),
        It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("does not exist")),
        It.IsAny<Exception>(),
        (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
    Times.Once);
    }

    [Test]
    public async Task FileNameNotFound()
    {
        // Arrange
        var jobData = new JobDataMap();
        var context = new Mock<IJobExecutionContext>();
        context.Setup(x => x.JobDetail.JobDataMap).Returns(jobData);

        //Act
        await _job.Execute(context.Object);

        //Assert
        _jobLogger.Verify(
    m => m.Log(
        LogLevel.Error,
        It.IsAny<EventId>(),
        It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("File path not found in the job data")),
        It.IsAny<Exception>(),
        (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
    Times.Once);
    }

    [Test]
    public async Task FileNameEmpty()
    {
        // Arrange
        var jobData = new JobDataMap();
        jobData.Add("fileName", "");
        var context = new Mock<IJobExecutionContext>();
        context.Setup(x => x.JobDetail.JobDataMap).Returns(jobData);

        //Act
        await _job.Execute(context.Object);

        //Assert
        _jobLogger.Verify(
            m => m.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("File path is empty")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
            Times.Once);
    }

}