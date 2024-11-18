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
        private BaseJob _baseJob;
        public QuartzJob(BaseJob baseJob)
        {
            _baseJob = baseJob;
        }
        public Task Execute(IJobExecutionContext context)
        {
            var jobData = context.JobDetail.JobDataMap;
            var args = jobData.ToDictionary(x => x.Key, x => x.Value.ToString());
            args = args.Where(x => x.Value is string).ToDictionary(x => x.Key, x => x.Value);

            return _baseJob.ExecuteAsync(args!);
        }
    }
}
