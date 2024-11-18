using OS.Data.Options;
using OS.Services.Codec;
using static OS.API.Extensions.StorageExtension;

namespace OS.API.Extensions
{
    public static class TranscoderExtension
    {
        public static IServiceCollection AddTranscoder(this IServiceCollection services)
        {
            var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
            if (configuration == null)
            {
                throw new Exception("Configuration is required");
            }

            TranscodeSettings transcodeSettings = new();
            configuration.GetSection("TranscodeSettings").Bind(transcodeSettings);

            services.AddSingleton<ITranscoder, FfmpegTranscoder>(x =>
                ActivatorUtilities.CreateInstance<FfmpegTranscoder>(x, transcodeSettings));

            return services;
        }
    }
}
