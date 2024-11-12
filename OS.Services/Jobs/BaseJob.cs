using Microsoft.Extensions.Logging;
using OS.Services.Storage;
using Quartz;

namespace OS.Services.Jobs;

public abstract class BaseJob(
    ILogger<BaseJob> logger,
    IStorageService storageService,
    ITempStorageService tempStorageService,
    IRepository.IRepository repository) : IJob
{
    protected readonly ILogger<BaseJob> Logger = logger;
    protected readonly IStorageService StorageService = storageService;
    protected readonly ITempStorageService TempStorageService = tempStorageService;
    protected readonly IRepository.IRepository Repository = repository;

    public async Task Execute(IJobExecutionContext context)
    {
        Logger.LogInformation($"Executing job {context.JobDetail.Key.Name}");
        var result = await ExecuteJob(context);
        Logger.LogInformation($"Job {context.JobDetail.Key.Name} executed with result {result}");
    }

    protected abstract Task<bool> ExecuteJob(IJobExecutionContext context);
}