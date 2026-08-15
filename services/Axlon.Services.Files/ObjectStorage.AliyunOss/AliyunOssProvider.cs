using AlibabaCloud.OSS.V2.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OSS = AlibabaCloud.OSS.V2;

namespace Axlon.Services.Files.ObjectStorage.AliyunOss;

public sealed class AliyunOssProvider : IObjectStorageProvider, IDisposable
{
    private const string ServerSideEncryption = "AES256";
    private readonly OSS.Client _client;
    private readonly AliyunOssOptions _options;

    public AliyunOssProvider(IOptions<AliyunOssOptions> options)
    {
        _options = options.Value;
        var configuration = OSS.Configuration.LoadDefault();
        configuration.Region = _options.Region;
        configuration.Endpoint = _options.Endpoint;
        configuration.UseCName = _options.UseCName;
        // OSS V4 是当前推荐签名方式，也是浏览器预签名直传的统一实现基础。
        configuration.SignatureVersion = "v4";
        configuration.CredentialsProvider = CreateCredentialsProvider(_options);
        _client = new OSS.Client(configuration);
    }

    public string Name => _options.ProviderName;
    public string Bucket => _options.Bucket;

    public async Task<ObjectUploadSession> InitializeUploadAsync(
        ObjectUploadIntent intent,
        CancellationToken cancellationToken = default)
    {
        EnsureAddress(intent.Address);
        if (intent.Mode == ObjectUploadMode.Single)
            return new ObjectUploadSession(null);

        // 分片 UploadId 属于服务端会话状态，不能交由客户端自行初始化或替换。
        var result = await _client.InitiateMultipartUploadAsync(new InitiateMultipartUploadRequest
        {
            Bucket = intent.Address.Bucket,
            Key = intent.Address.ObjectKey,
            ContentType = intent.ContentType,
            ServerSideEncryption = ServerSideEncryption,
            ForbidOverwrite = true
        }, cancellationToken: cancellationToken);

        return new ObjectUploadSession(result.UploadId);
    }

    public Task<SignedObjectRequest> SignUploadAsync(
        ObjectUploadIntent intent,
        string? providerUploadId,
        int? partNumber,
        TimeSpan lifetime,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureAddress(intent.Address);
        var expiresAt = DateTime.UtcNow.Add(lifetime);

        PresignResult result;
        if (intent.Mode == ObjectUploadMode.Single)
        {
            if (partNumber is not null || providerUploadId is not null)
                throw new ArgumentException("Single uploads cannot include multipart identifiers.");

            // Content-Type、长度、禁止覆盖和服务端加密都参与签名，客户端必须原样携带返回头。
            result = _client.Presign(new PutObjectRequest
            {
                Bucket = intent.Address.Bucket,
                Key = intent.Address.ObjectKey,
                ContentType = intent.ContentType,
                ContentLength = intent.ContentLength,
                ServerSideEncryption = ServerSideEncryption,
                ForbidOverwrite = true
            }, expiresAt);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(providerUploadId) || partNumber is null)
                throw new ArgumentException("Multipart uploads require an upload ID and part number.");

            result = _client.Presign(new UploadPartRequest
            {
                Bucket = intent.Address.Bucket,
                Key = intent.Address.ObjectKey,
                UploadId = providerUploadId,
                PartNumber = partNumber.Value,
                ContentLength = intent.ContentLength
            }, expiresAt);
        }

        return Task.FromResult(ToSignedRequest(result));
    }

    public async Task CompleteUploadAsync(
        ObjectUploadIntent intent,
        string providerUploadId,
        IReadOnlyList<CompletedObjectPart> parts,
        CancellationToken cancellationToken = default)
    {
        EnsureAddress(intent.Address);
        await _client.CompleteMultipartUploadAsync(new CompleteMultipartUploadRequest
        {
            Bucket = intent.Address.Bucket,
            Key = intent.Address.ObjectKey,
            UploadId = providerUploadId,
            CompleteMultipartUpload = new CompleteMultipartUpload
            {
                Parts = parts.Select(part => new UploadPart
                {
                    PartNumber = part.PartNumber,
                    ETag = NormalizeETag(part.ETag)
                }).ToList()
            }
        }, cancellationToken: cancellationToken);
    }

    public async Task AbortUploadAsync(
        ObjectAddress address,
        string providerUploadId,
        CancellationToken cancellationToken = default)
    {
        EnsureAddress(address);
        await _client.AbortMultipartUploadAsync(new AbortMultipartUploadRequest
        {
            Bucket = address.Bucket,
            Key = address.ObjectKey,
            UploadId = providerUploadId
        }, cancellationToken: cancellationToken);
    }

    public async Task<ObjectInfo?> GetInfoAsync(
        ObjectAddress address,
        CancellationToken cancellationToken = default)
    {
        EnsureAddress(address);
        try
        {
            var result = await _client.HeadObjectAsync(new HeadObjectRequest
            {
                Bucket = address.Bucket,
                Key = address.ObjectKey
            }, cancellationToken: cancellationToken);
            return new ObjectInfo(result.ContentLength ?? 0, result.ContentType ?? "application/octet-stream", result.ETag);
        }
        catch (Exception exception) when (IsNotFound(exception))
        {
            return null;
        }
    }

    public async Task<byte[]> ReadRangeAsync(
        ObjectAddress address,
        long offset,
        int length,
        CancellationToken cancellationToken = default)
    {
        EnsureAddress(address);
        ArgumentOutOfRangeException.ThrowIfNegative(offset);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);

        var result = await _client.GetObjectAsync(new GetObjectRequest
        {
            Bucket = address.Bucket,
            Key = address.ObjectKey,
            Range = $"bytes={offset}-{checked(offset + length - 1)}"
        }, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken: cancellationToken);

        await using var body = result.Body ?? throw new InvalidOperationException("OSS returned an empty response body.");
        using var buffer = new MemoryStream(Math.Min(length, 4096));
        await body.CopyToAsync(buffer, cancellationToken);
        return buffer.ToArray();
    }

    public Task<SignedObjectRequest> SignDownloadAsync(
        ObjectAddress address,
        string fileName,
        ObjectDownloadDisposition disposition,
        TimeSpan lifetime,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureAddress(address);
        var value = disposition == ObjectDownloadDisposition.Inline ? "inline" : "attachment";
        var encodedName = Uri.EscapeDataString(fileName);
        var result = _client.Presign(new GetObjectRequest
        {
            Bucket = address.Bucket,
            Key = address.ObjectKey,
            ResponseContentDisposition = $"{value}; filename*=UTF-8''{encodedName}"
        }, DateTime.UtcNow.Add(lifetime));
        return Task.FromResult(ToSignedRequest(result));
    }

    public async Task DeleteAsync(ObjectAddress address, CancellationToken cancellationToken = default)
    {
        EnsureAddress(address);
        await _client.DeleteObjectAsync(new DeleteObjectRequest
        {
            Bucket = address.Bucket,
            Key = address.ObjectKey
        }, cancellationToken: cancellationToken);
    }

    public async Task CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        await _client.GetBucketInfoAsync(new GetBucketInfoRequest { Bucket = Bucket },
            cancellationToken: cancellationToken);
    }

    public void Dispose() => _client.Dispose();

    private static OSS.Credentials.ICredentialsProvider CreateCredentialsProvider(AliyunOssOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.AccessKeyId))
            return new OSS.Credentials.EnvironmentVariableCredentialsProvider();

        return string.IsNullOrWhiteSpace(options.SecurityToken)
            ? new OSS.Credentials.StaticCredentialsProvider(options.AccessKeyId, options.AccessKeySecret!)
            : new OSS.Credentials.StaticCredentialsProvider(options.AccessKeyId, options.AccessKeySecret!, options.SecurityToken);
    }

    private SignedObjectRequest ToSignedRequest(PresignResult result)
    {
        var rawExpiration = result.Expiration ?? throw new InvalidOperationException("OSS did not return a presign expiration.");
        var expiration = rawExpiration.Kind == DateTimeKind.Utc
            ? rawExpiration
            : rawExpiration.ToUniversalTime();
        return new SignedObjectRequest(
            new Uri(result.Url ?? throw new InvalidOperationException("OSS did not return a presigned URL."), UriKind.Absolute),
            new DateTimeOffset(expiration),
            result.SignedHeaders is null
                ? new Dictionary<string, string>()
                : new Dictionary<string, string>(result.SignedHeaders, StringComparer.OrdinalIgnoreCase));
    }

    private void EnsureAddress(ObjectAddress address)
    {
        if (!Name.Equals(address.Provider, StringComparison.OrdinalIgnoreCase)
            || !Bucket.Equals(address.Bucket, StringComparison.Ordinal))
            throw new InvalidOperationException("Object address does not belong to this OSS provider.");
    }

    // 浏览器读到的 ETag 响应头包含引号，OSS 完成分片时也要求 XML 中保留该值，不能擅自去引号。
    private static string NormalizeETag(string value) => value.Trim();

    private static bool IsNotFound(Exception exception)
    {
        for (var current = exception; current is not null; current = current.InnerException)
        {
            var statusCode = current.GetType().GetProperty("StatusCode")?.GetValue(current);
            if (statusCode is int integer && integer == 404) return true;
            if (statusCode?.ToString() == "NotFound" || statusCode?.ToString() == "404") return true;
            if (current.Message.Contains("NoSuchKey", StringComparison.OrdinalIgnoreCase)) return true;
        }
        return false;
    }
}
