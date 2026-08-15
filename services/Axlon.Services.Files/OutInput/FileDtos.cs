using Axlon.Services.Contracts.Models.Files;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Axlon.Services.Files.OutInput;

public sealed class BeginFileUploadRequest
{
    [Required, StringLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required, StringLength(128)]
    public string ContentType { get; set; } = string.Empty;

    [Range(1, long.MaxValue)]
    public long Size { get; set; }

    [Required]
    public string Visibility { get; set; } = FileVisibilities.Private;
}

public sealed record SignedRequestOutput(
    string Url,
    DateTime ExpiresAt,
    IReadOnlyDictionary<string, string> Headers);

public sealed record BeginFileUploadOutput(
    long FileId,
    string Mode,
    long? PartSize,
    int? PartCount,
    DateTime SessionExpiresAt,
    SignedRequestOutput? Upload);

public sealed class SignFilePartsRequest
{
    [Required, MinLength(1), MaxLength(50)]
    public int[] PartNumbers { get; set; } = [];
}

public sealed record SignedFilePartOutput(int PartNumber, SignedRequestOutput Request);

public sealed class CompleteFileUploadRequest
{
    public CompletedFilePartInput[] Parts { get; set; } = [];
}

public sealed class CompletedFilePartInput
{
    [Range(1, 10_000)]
    public int PartNumber { get; set; }

    [Required, StringLength(256)]
    public string ETag { get; set; } = string.Empty;
}

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

public sealed class CreateDownloadUrlRequest
{
    [Required]
    public string Disposition { get; set; } = "attachment";
}

public sealed record DownloadUrlOutput(
    string Url,
    DateTime ExpiresAt,
    string FileName,
    string ContentType);
