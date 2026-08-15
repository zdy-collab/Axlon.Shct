namespace Axlon.Services.Files.ObjectStorage.Local;

public sealed class LocalObjectStorageOptions
{
    public const string SectionName = "Axlon:Files:Providers:local";

    public string ProviderName { get; set; } = "local";
    public string Bucket { get; set; } = "wwwroot";
    public string RootPath { get; set; } = "wwwroot/files";
    public string UploadPath { get; set; } = "wwwroot/.uploads";
    public string? PublicBaseUrl { get; set; }
}
