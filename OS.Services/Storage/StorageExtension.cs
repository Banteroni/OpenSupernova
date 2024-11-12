using Microsoft.Extensions.DependencyInjection;

namespace OS.Services.Storage;

public static class StorageExtension 
{
    public static IServiceCollection AddStorageServices(this IServiceCollection services)
    {
        return services;
    }
}