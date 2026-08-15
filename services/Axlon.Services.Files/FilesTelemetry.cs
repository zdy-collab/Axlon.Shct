using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Axlon.Services.Files;

internal static class FilesTelemetry
{
    public const string SourceName = "Axlon.Services.Files";
    public static readonly ActivitySource ActivitySource = new(SourceName);
    public static readonly Meter Meter = new(SourceName);
    public static readonly Counter<long> SessionsCreated = Meter.CreateCounter<long>("files.upload.sessions.created");
    public static readonly Counter<long> UploadsCompleted = Meter.CreateCounter<long>("files.uploads.completed");
    public static readonly Counter<long> UploadsFailed = Meter.CreateCounter<long>("files.uploads.failed");
    public static readonly Counter<long> UrlsSigned = Meter.CreateCounter<long>("files.urls.signed");
    public static readonly Counter<long> ObjectsPurged = Meter.CreateCounter<long>("files.objects.purged");
    public static readonly Counter<long> CleanupFailures = Meter.CreateCounter<long>("files.cleanup.failures");
}
