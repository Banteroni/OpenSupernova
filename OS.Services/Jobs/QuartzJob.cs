using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.Services.Jobs
{
    public class QuartzJob : IJob
    {
        private List<BaseJob> _jobs;

        public QuartzJob(List<BaseJob> jobs)
        {
            _jobs = jobs;
        }
        public Task Execute(IJobExecutionContext context)
        {
            var jobData = context.JobDetail.JobDataMap;
            var args = jobData.ToDictionary(x => x.Key, x => x.Value.ToString());
            args = args.Where(x => x.Value is string).ToDictionary(x => x.Key, x => x.Value);

            var jobName = context.JobDetail.Key.Name;
            var baseJob = _jobs.FirstOrDefault(x => x.GetType().Name == jobName);
            if (baseJob == null)
            {
                throw new Exception($"Job {jobName} not found");
            }
            return baseJob.ExecuteAsync(args!);
        }
    }
}
