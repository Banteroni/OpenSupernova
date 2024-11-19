using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.Services.Jobs
{
    public class QuartzJobManager : IJobManager
    {
        private readonly IScheduler _scheduler;
        private readonly ILogger<QuartzJobManager> _logger;
        public QuartzJobManager(IScheduler scheduler, ILogger<QuartzJobManager> logger)
        {
            _logger = logger;
            _scheduler = scheduler;
        }

        public Task Fire(BaseJob job, Dictionary<string, string>? args = null)
        {
            var trigger = TriggerBuilder.Create()
                .StartNow()
                .Build();
            return ScheduleJob(job, trigger, args);
        }

        public Task FireAt(BaseJob job, DateTime time, Dictionary<string, string>? args = null)
        {
            var trigger = TriggerBuilder.Create()
                .StartAt(time)
                .Build();
            return ScheduleJob(job, trigger, args);
        }

        public Task FireEvery(BaseJob job, string expr, Dictionary<string, string>? args = null)
        {
            var trigger = TriggerBuilder.Create()
                .WithCronSchedule(expr)
                .Build();
            return ScheduleJob(job, trigger, args);
        }

        public Task<bool> IsJobRunning<T>()
        {
            var jobKey = new JobKey(typeof(T).Name);
            return _scheduler.CheckExists(jobKey);
        }


        private async Task ScheduleJob(BaseJob job, ITrigger trigger, Dictionary<string, string>? args)
        {
            try
            {
                var jobName = job.GetType();
                var jobData = new JobDataMap(args ?? new Dictionary<string, string>());

                var jobDetail = JobBuilder.Create<QuartzJob>()
                    .WithIdentity(job.GetType().Name)
                    .UsingJobData(jobData)
                    .Build();

                // Check if job already exists
                if (await _scheduler.CheckExists(jobDetail.Key))
                {
                    await _scheduler.DeleteJob(jobDetail.Key);
                }
                await _scheduler.ScheduleJob(jobDetail, trigger);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to schedule job {job.GetType().Name}, {ex}");
            }
        }
    }
}