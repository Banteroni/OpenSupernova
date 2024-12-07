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

            var settings = configuration!.GetSection("TranscodeSettings").Get<TranscodeSettings>();

            if (settings == null)
            {
                settings = new TranscodeSettings()
                {
                    BitrateKbps = 128,
                    SampleRate = 48000,
                    Channels = 2,
                    CompressionLevel = 10
                };
            }

            services.AddSingleton<ITranscoder, FfmpegTranscoder>(x =>
            ActivatorUtilities.CreateInstance<FfmpegTranscoder>(x, settings));

            return services;
        }
    }
}
