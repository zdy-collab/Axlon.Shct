namespace Axlon.Services.Files.ObjectStorage.AliyunOss;

public sealed class AliyunOssOptions
{
    public const string SectionName = "Axlon:Files:Providers:oss";

    public bool Enabled { get; set; } = true;
    public string ProviderName { get; set; } = "oss";
    public string Region { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string Bucket { get; set; } = string.Empty;
    public bool UseCName { get; set; }
    public string? AccessKeyId { get; set; }
    public string? AccessKeySecret { get; set; }
    public string? SecurityToken { get; set; }
}
