using Microsoft.Extensions.Logging;
using OS.Services.Storage;
using Quartz;

namespace OS.Services.Jobs;

public class ImportTracksJob(ILogger<ImportTracksJob> logger, IStorageService storageService, IRepository.IRepository repository) : BaseJob(logger, storageService, repository)
{
    public override string Name { get; init; } = nameof(ImportTracksJob);
    public override string Group { get; init; } = "ImportGroup";

    public override async Task Execute(IJobExecutionContext context)
    {
        
    }
}