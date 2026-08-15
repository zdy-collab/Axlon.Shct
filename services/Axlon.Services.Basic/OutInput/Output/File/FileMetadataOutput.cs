namespace Axlon.Services.Basic.OutInput.Output.File
{
    public sealed record FileMetadataOutput(
        long Id,
        string OriginalName,
        string MediaKind,
        string ContentType,
        long Size,
        string Visibility,
        string Status,
        DateTime CreatedAt,
        DateTime? ReadyAt);
}
