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
    public class DatabaseCleanup(IStorageService storageService, IRepository repository, ISchedulerFactory schedulerFactory) : IJob
    {

        private readonly IStorageService _storageService = storageService;
        private readonly IRepository _repository = repository;
        private readonly ISchedulerFactory _schedulerFactory = schedulerFactory;

        public static readonly JobKey Key = new JobKey(nameof(DatabaseCleanup), "maintenance");

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

            var musicEntities = entities.Where(e => e.Type == StoredEntityType.Origin || e.Type == StoredEntityType.Stream);
            var entitiesNotInStorage = musicEntities.Where(e => !files.Contains(e.Id.ToString()));
            foreach (var entity in entitiesNotInStorage)
            {
                await _repository.DeleteWhereAsync<StoredEntity>(x => entity.Artist == x.Artist && entity.Album == x.Album && entity.Track == x.Track, false);
                if (entity.Artist != null)
                {
                    await _repository.DeleteWhereAsync<Artist>(x => entity.Artist.Id == x.Id, false);
                }
                else if (entity.Album != null)
                {
                    await _repository.DeleteWhereAsync<Album>(x => entity.Album.Id == x.Id, false);
                }
                else if (entity.Track != null)
                {
                    await _repository.DeleteWhereAsync<Track>(x => entity.Track.Id == x.Id, false);
                }
            }
            await _repository.SaveChangesAsync();
        }
    }
}