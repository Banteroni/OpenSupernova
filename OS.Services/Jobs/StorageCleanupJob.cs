using OS.Data.Models;
using OS.Services.Repository;
using OS.Services.Storage;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.Services.Jobs
{
    public class StorageCleanupJob(IStorageService storageService, IRepository repository, IScheduler scheduler) : IJob
    {

        private readonly IStorageService _storageService = storageService;
        private readonly IRepository _repository = repository;
        private readonly IScheduler _scheduler = scheduler;

        public static readonly JobKey Key = new JobKey(nameof(StorageCleanupJob), "maintenance");

        public async Task Execute(IJobExecutionContext context)
        {
            var isImporting = await _scheduler.CheckExists(ImportTracksJob.Key);
            if (isImporting)
            {
                return;
            }
            var files = await _storageService.ListAllFilesAsync();
            var tracks = await _repository.GetListAsync<Track>();
            var albums = await _repository.GetListAsync<Album>();

            // Get all files that are not used by any track, or albumid_cover
            var filesToDelete = files.Where(f => !tracks.Any(t => $"{t.Id}.opus" == f) || !albums.Any(x => $"{x.Id.ToString()}_cover" == f)).ToList();
            foreach (var file in filesToDelete)
            {
                await _storageService.DeleteFileAsync(file);
            }
        }
    }
}
