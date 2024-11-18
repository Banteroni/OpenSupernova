using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.Services.Jobs
{
    public abstract class BaseJob
    {
        public abstract Task ExecuteAsync(Dictionary<string, string>? args = null);

        public virtual Task ScheduleAtStartupAsync()
        {
             return Task.CompletedTask;
        }

    }
}
