using OS.Services.Jobs;
using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace OS.API.Extensions
{
    public static class ScheduleJobsExtension
    {
        public static IServiceCollection ScheduleJobs(this IServiceCollection services)
        {
            var jobManager = services.BuildServiceProvider().GetService<IJobManager>();
            var otherProjectAssembly = Assembly.Load("OS.Services");
            var jobTypes = otherProjectAssembly.GetTypes()
                .Where(type => typeof(BaseJob).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                .ToList();

            List<BaseJob> jobs = [];
            foreach (var jobType in jobTypes)
            {

                //Execute the ScheduleAtStartup method of the job
                var job = ActivatorUtilities.CreateInstance(services.BuildServiceProvider(), jobType) as BaseJob;
                if (job != null)
                {
                    jobs.Add(job);
                    job.ScheduleAtStartup();
                }
            }
            services.AddScoped<List<BaseJob>>(x => jobs);

            return services;
        }
    }
}
