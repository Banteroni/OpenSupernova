using OS.Services.Repository;
using OS.Services.Storage;
using Quartz;


namespace OS.Services.Jobs
{
    public class TemporaryStorageCleanupJob : IJob
    {
        private ITempStorageService _tempStorageService;
        private IRepository _repository;
        private ISchedulerFactory _schedulerFactory;

        public static readonly JobKey Key = new JobKey(nameof(TemporaryStorageCleanupJob), "maintenance");
        public TemporaryStorageCleanupJob(IRepository repository, ITempStorageService tempStorageService, ISchedulerFactory schedulerFactory)
        {

            _tempStorageService = tempStorageService;
            _repository = repository;
            _schedulerFactory = schedulerFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            // check if job ImportStorageJob is running
            var importJob = await scheduler.GetJobDetail(ImportTracksJob.Key);
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
