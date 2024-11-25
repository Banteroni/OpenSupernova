using OS.Data.Options;
using OS.Services.Storage;

namespace OS.API.Extensions;

public static class StorageExtension
{
    public static class StoragePurpose
    {
        public const string StorageSettings = "StorageSettings";
        public const string TemporaryStorageSettings = "TemporaryStorageSettings";
    }

    public static IServiceCollection AddStorage(this IServiceCollection services)
    {
        var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
        if (configuration == null)
        {
            throw new Exception("Configuration is required");
        }

        Dictionary<string, BaseStorageSettings?> storageSettings = new()
        {
            { StoragePurpose.StorageSettings, GetStorageFromKey(configuration, StoragePurpose.StorageSettings) },
            { StoragePurpose.TemporaryStorageSettings, GetStorageFromKey(configuration, StoragePurpose.TemporaryStorageSettings) },
        };
        foreach (var (key, value) in storageSettings)
        {
            if (value is LocalStorageSettings localSettingsKey)
            {
                if (localSettingsKey.Path == null)
                {
                    throw new Exception("Path is required");
                }
                Directory.CreateDirectory(localSettingsKey.Path);
            }

            switch (key)
            {
                case StoragePurpose.StorageSettings:
                    services.AddSingleton<IStorageService>(LoadStorage(services, value));
                    break;
                case StoragePurpose.TemporaryStorageSettings:
                    if (value == null)
                    {
                        var localStorage = new LocalStorageSettings
                        {
                            Path = Path.Combine(Directory.GetCurrentDirectory(), "temp")
                        };
                        var tempStorage = (ITempStorageService)LoadStorage(services, localStorage);
                        services.AddSingleton<ITempStorageService>(tempStorage);
                    }
                    else
                    {
                        services.AddSingleton<ITempStorageService>((ITempStorageService)LoadStorage(services, value));
                    }
                    break;
                default:
                    break;
            }
        }
        return services;
    }

    private static IStorageService LoadStorage(IServiceCollection services, BaseStorageSettings? value)
    {
        IStorageService service;
        switch (value)
        {
            case LocalStorageSettings localSettings:
                if (localSettings.Path == null)
                {
                    throw new Exception("Path is required");
                }
                service = ActivatorUtilities.CreateInstance<LocalStorageService>(services.BuildServiceProvider(), localSettings.Path);
                return service;
            case AzureStorageSettings azureSettings:
                if (azureSettings.ConnectionString == null || azureSettings.ContainerName == null)
                {
                    throw new Exception("ConnectionString and ContainerName are required");
                }
                service = ActivatorUtilities.CreateInstance<AzureStorageService>(services.BuildServiceProvider(), azureSettings.ConnectionString, azureSettings.ContainerName);
                return service;
            default:
                throw new NotSupportedException("Storage type not supported");
        }
    }

    private static BaseStorageSettings? GetStorageFromKey(IConfiguration configuration, string key)
    {
        if (configuration == null)
        {
            throw new Exception("Configuration is required");
        }

        var storageSettings = configuration.GetSection(key).Get<BaseStorageSettings>();
        if (storageSettings == null)
        {
            return null;
        }

        switch (storageSettings.Type)
        {
            case "local":
                var localSettings = configuration.GetSection(key).Get<LocalStorageSettings>();
                if (localSettings == null)
                {
                    throw new Exception($"{key} was found, but the object does not match the expected type");
                }

                return localSettings;
            case "azure":
                var azureSettings = configuration.GetSection(key).Get<AzureStorageSettings>();
                if (azureSettings == null)
                {
                    throw new Exception($"{key} was found, but the object does not match the expected type");
                }

                return azureSettings;
            default:
                throw new NotSupportedException("Storage type not supported");
        }
    }
}