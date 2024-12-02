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
    public class StorageCleanupJob(IStorageService storageService, IRepository repository, ISchedulerFactory schedulerFactory) : IJob
    {

        private readonly IStorageService _storageService = storageService;
        private readonly IRepository _repository = repository;
        private readonly ISchedulerFactory _schedulerFactory = schedulerFactory;

        public static readonly JobKey Key = new JobKey(nameof(StorageCleanupJob), "maintenance");

        public async Task Execute(IJobExecutionContext context)
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            var isImporting = await scheduler.CheckExists(ImportTracksJob.Key);
            if (isImporting)
            {
                return;
            }
            var files = await _storageService.ListAllFilesAsync();
            var entities = await _repository.GetAllAsync<StoredEntity>();

            foreach (var file in files)
            {
                if (!entities.Any(e => e.Id.ToString() == file))
                {
                    await _storageService.DeleteFileAsync(file);
                }
            }
        }
    }
}