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

    private static void LoadStorageServiceFromGivenSetting(IServiceCollection services, BaseStorageSettings settings,
        string storagePurpose)
    {
        if (settings == null)
        {
            throw new Exception("Settings is required");
        }

        var storageType = settings.Type;
        switch (storageType)
        {
            case "local":
                if (settings is not LocalStorageSettings localSettings || localSettings.Path == null)
                {
                    throw new Exception("Settings does not match the expected type");
                }
                if (storagePurpose == StoragePurpose.TemporaryStorageSettings)
                {
                    services.AddSingleton<ITempStorageService, LocalStorageService>(x =>
                        ActivatorUtilities.CreateInstance<LocalStorageService>(x, localSettings.Path));
                }
                else
                {
                    services.AddSingleton<IStorageService, LocalStorageService>(x =>
                        ActivatorUtilities.CreateInstance<LocalStorageService>(x, localSettings.Path));
                }

                break;
            case "azure":
                var azureSettings = settings as AzureStorageSettings;
                services.AddSingleton<IStorageService, AzureStorageService>(x =>
                {
                    if (azureSettings is { ConnectionString: not null, ContainerName: not null })
                        return ActivatorUtilities.CreateInstance<AzureStorageService>(x,
                            azureSettings.ConnectionString,
                            azureSettings.ContainerName);
                    throw new Exception("ConnectionString and ContainerName are required");
                });
                break;
            default:
                throw new NotSupportedException("Storage type not supported");
        }
    }

    private static BaseStorageSettings GetStorageFromKey(IConfiguration configuration, string key)
    {
        if (configuration == null)
        {
            throw new Exception("Configuration is required");
        }

        var storageSettings = configuration.GetSection(key).Get<BaseStorageSettings>();
        if (storageSettings == null)
        {
            throw new Exception($"{key} not found in configuration");
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

    public static IServiceCollection AddStorage(this IServiceCollection services)
    {
        var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
        if (configuration == null)
        {
            throw new Exception("Configuration is required");
        }

        var storageSettings = GetStorageFromKey(configuration, StoragePurpose.StorageSettings);
        LoadStorageServiceFromGivenSetting(services, storageSettings, StoragePurpose.StorageSettings);

        var tempStorageSettings = GetStorageFromKey(configuration, StoragePurpose.TemporaryStorageSettings);
        LoadStorageServiceFromGivenSetting(services, tempStorageSettings, StoragePurpose.TemporaryStorageSettings);

        return services;
    }
}