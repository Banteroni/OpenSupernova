namespace OS.Data.Options;

public class AzureStorageSettings : BaseStorageSettings
{
    public string? ConnectionString { get; set; }
    public string? ContainerName { get; set; }
}