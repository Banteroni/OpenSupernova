﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.Services.Jobs
{
    public interface IJobManager
    {
        public Task Fire(BaseJob job, Dictionary<string, string>? args = null);

        public Task FireAt(BaseJob job, DateTime time, Dictionary<string, string>? args = null);

        public Task Schedule(BaseJob job, TimeSpan interval, Dictionary<string, string>? args = null);
    }
}