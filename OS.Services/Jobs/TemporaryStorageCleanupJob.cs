using OS.Services.Repository;
using OS.Services.Storage;


namespace OS.Services.Jobs
{
    public class TemporaryStorageCleanupJob : BaseJob
    {
        private ITempStorageService _tempStorageService;
        public TemporaryStorageCleanupJob(IJobManager jobManager, IRepository repository, ITempStorageService tempStorageService) : base(jobManager, repository)
        {
            _tempStorageService = tempStorageService;
        }

        public override async Task ExecuteAsync(Dictionary<string, string>? args = null)
        {
            var isImportTracksJobRunning = await _jobManager.IsJobRunning<ImportTracksJob>();
            if (isImportTracksJobRunning)
            {
                return;
            }
            var files = await _tempStorageService.ListAllFilesAsync();
            foreach (var file in files)
            {
                await _tempStorageService.DeleteFileAsync(file);
            }


        }

        public override void ScheduleAtStartup()
        {
            // cron every hour
            var cronExpr = "0 0 * ? * *";
            _jobManager.FireEvery(this, cronExpr);
        }
    }
}
