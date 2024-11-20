using Quartz;

namespace OS.API.Extensions
{
    public static class SchedulerExtension
    {
        public static IServiceCollection AddScheduler(this IServiceCollection services)
        {
            // Get scheduler factory
            var schedulerFactory = services.BuildServiceProvider().GetService<ISchedulerFactory>();
            if (schedulerFactory == null)
            {
                throw new Exception("SchedulerFactory is required");
            }
            var scheduler = schedulerFactory.GetScheduler().Result;
            scheduler.Start();
            services.AddSingleton(scheduler);

            return services;
        }
    }
}
