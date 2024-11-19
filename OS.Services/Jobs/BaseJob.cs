using OS.Services.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.Services.Jobs
{
    public abstract class BaseJob
    {
        internal IJobManager _jobManager;
        internal IRepository _repository;
        public BaseJob(IJobManager jobManager, IRepository repository)
        {
            _jobManager = jobManager;
            _repository = repository;
        }
        public abstract Task ExecuteAsync(Dictionary<string, string>? args = null);

        public virtual void ScheduleAtStartup()
        {
        }

    }
}
