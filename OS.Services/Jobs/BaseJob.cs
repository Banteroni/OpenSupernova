using Microsoft.Extensions.Logging;
using OS.Services.Storage;
using Quartz;

namespace OS.Services.Jobs;

public abstract class BaseJob(ILogger<BaseJob> logger, IStorageService storageService, IRepository.IRepository repository) : IJob
{
    public abstract string Name { get; init; }
    public abstract string Group { get; init; }
    protected readonly ILogger<BaseJob> Logger  = logger;
    protected readonly IStorageService StorageService = storageService;
    protected readonly IRepository.IRepository Repository = repository;
    public abstract Task Execute(IJobExecutionContext context);
}