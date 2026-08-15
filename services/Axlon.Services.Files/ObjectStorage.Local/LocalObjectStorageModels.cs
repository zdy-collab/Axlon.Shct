using System;
using System.Text.Json.Serialization;

namespace Axlon.Services.Files.ObjectStorage.Local;

internal static class LocalTokenOperations
{
    public const string Upload = "upload";
    public const string Download = "download";
}

internal sealed record LocalSignedToken
{
    public required string Operation { get; init; }
    public required string Provider { get; init; }
    public required string Bucket { get; init; }
    public required string ObjectKey { get; init; }
    public required DateTimeOffset ExpiresAt { get; init; }
    public string? ContentType { get; init; }
    public long? ContentLength { get; init; }
    public string? ProviderUploadId { get; init; }
    public int? PartNumber { get; init; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ObjectDownloadDisposition? Disposition { get; init; }
    public string? FileName { get; init; }
}

internal sealed record LocalDownloadFile(
    string PhysicalPath,
    string ContentType,
    string FileName,
    ObjectDownloadDisposition Disposition);

internal sealed class LocalObjectStorageRequestException(int statusCode, string message)
    : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}
