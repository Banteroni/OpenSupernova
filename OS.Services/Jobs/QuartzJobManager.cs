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
        public QuartzJobManager(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public Task Fire(BaseJob job, Dictionary<string, string>? args = null)
        {
            return ScheduleJob(job, TimeSpan.Zero, args);
        }

        public Task FireAt(BaseJob job, DateTime time, Dictionary<string, string>? args = null)
        {
            return ScheduleJob(job, time - DateTime.Now, args);
        }

        public Task Schedule(BaseJob job, TimeSpan interval, Dictionary<string, string>? args = null)
        {

            return ScheduleJob(job, interval, args);
        }


        private async Task ScheduleJob(BaseJob job, TimeSpan interval, Dictionary<string, string>? args = null)
        {

            var jobData = new JobDataMap(args ?? new Dictionary<string, string>());
            var jobDetail = JobBuilder.Create(job.GetType())
                .WithIdentity(job.GetType().Name)
                .UsingJobData(jobData)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity(job.GetType().Name + "Trigger")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithInterval(interval)
                    .RepeatForever())
                .Build();

            await _scheduler.ScheduleJob(jobDetail, trigger);
        }
    }
}
