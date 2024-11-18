using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.Services.Jobs
{
    public class PlaylistJob : BaseJob
    {
        public override Task ExecuteAsync(Dictionary<string, string>? args = null)
        {
            throw new NotImplementedException();
        }

        public override Task ScheduleAtStartupAsync()
        {
            throw new NotImplementedException();
        }
    }
}
