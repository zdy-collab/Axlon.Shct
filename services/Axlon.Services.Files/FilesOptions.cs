namespace Axlon.Services.Files;

public sealed class FilesOptions
{
    public const string SectionName = "Axlon:Files";

    public string DefaultProvider { get; set; } = "local";
    public long MaxImageBytes { get; set; } = 20L * 1024 * 1024;
    public long MaxVideoBytes { get; set; } = 2L * 1024 * 1024 * 1024;
    public long MultipartThresholdBytes { get; set; } = 100L * 1024 * 1024;
    public long PartSizeBytes { get; set; } = 8L * 1024 * 1024;
    public int SignedUrlLifetimeMinutes { get; set; } = 15;
    public int UploadSessionLifetimeHours { get; set; } = 24;
    public int DeletedRetentionDays { get; set; } = 7;
    public int CleanupIntervalMinutes { get; set; } = 10;
    public int CleanupBatchSize { get; set; } = 100;
    public int CleanupLeaseMinutes { get; set; } = 5;
}
