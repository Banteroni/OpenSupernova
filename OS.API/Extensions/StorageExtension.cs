using OS.Data.Options;
using OS.Services.Storage;

namespace OS.API;

public static class StorageExtension
{
    public static IServiceCollection AddStorage(this IServiceCollection services, StorageSettings storageSettings, TemporaryStorageSettings temporaryStorageSettings)
    {
        
        services.AddSingleton<ITempStorageService, LocalStorageService>(x =>
            ActivatorUtilities.CreateInstance<LocalStorageService>(x, temporaryStorageSettings.Path ?? "temp"));

        var type = storageSettings.Type;
        switch (type)
        {
            case "local":
            {
                var path = storageSettings.Path;
                if (string.IsNullOrEmpty(path))
                {
                    throw new Exception("Path is required for local storage");
                }

                services.AddSingleton<IStorageService, LocalStorageService>(x =>
                    ActivatorUtilities.CreateInstance<LocalStorageService>(x, path));
                break;
            }
            case "azure":
            {
                var connectionString = storageSettings.ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new Exception("ConnectionString is required for azure storage");
                }

                services.AddSingleton<IStorageService, AzureStorageService>(x =>
                    ActivatorUtilities.CreateInstance<AzureStorageService>(x, connectionString));
                break;
            }
            default:
                throw new NotSupportedException("Storage type not supported");
        }

        return services;
    }
}