using OS.Services.Repository;
using OS.Services.Storage;
using Quartz;


namespace OS.Services.Jobs
{
    public class TemporaryStorageCleanupJob : IJob
    {
        private ITempStorageService _tempStorageService;
        private IRepository _repository;
        private IScheduler _scheduler;

        public static readonly JobKey Key = new JobKey(nameof(TemporaryStorageCleanupJob), "maintainance");
        public TemporaryStorageCleanupJob(IRepository repository, ITempStorageService tempStorageService, IScheduler scheduler)
        {
            _tempStorageService = tempStorageService;
            _repository = repository;
            _scheduler = scheduler;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            // check if job ImportStorageJob is running
            var importJob = await _scheduler.GetJobDetail(ImportTracksJob.Key);
            if (importJob != null)
            {
                return;
            }
            var files = await _tempStorageService.ListAllFilesAsync();
            foreach (var file in files)
            {
                await _tempStorageService.DeleteFileAsync(file);
            }
        }

    }
}
