using Microsoft.Extensions.Logging;
using Moq;
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
        var storageService = new LocalStorageService(storageLogger.Object, "archive");
        var tempStorageService = new LocalStorageService(storageLogger.Object, "temp");
        // read all bytes from the file
        var stream = new FileStream("./dummy.flac", FileMode.Open, FileAccess.Read, FileShare.Read);
        var buffer = new byte[stream.Length];
        stream.Read(buffer, 0, (int)stream.Length);
        await tempStorageService.SaveFileAsync(buffer, "dummy.flac");
        var repository = new MockRepository();
        _job = new ImportTracksJob(logger.Object, storageService, tempStorageService, repository);
    }
    
    
    [Test]
    public async Task ExecutionComplete()
    {
        var jobData = new JobDataMap();
        jobData.Add("fileName", "dummy.flac");
        var context = new Mock<IJobExecutionContext>();
        context.Setup(x => x.JobDetail.JobDataMap).Returns(jobData);
        await _job.Execute(context.Object);
    }
}