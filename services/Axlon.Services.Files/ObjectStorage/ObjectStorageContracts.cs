using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axlon.Services.Files.ObjectStorage;

/// <summary>
/// 区分单对象 PUT 与云厂商分片上传；阈值判断属于文件业务策略，不放在适配器中。
/// </summary>
public enum ObjectUploadMode
{
    Single,
    Multipart
}

public enum ObjectDownloadDisposition
{
    Inline,
    Attachment
}

/// <summary>对象的永久路由地址，随元数据保存，不能用当前默认 Provider 反推。</summary>
public sealed record ObjectAddress(string Provider, string Bucket, string ObjectKey);

public sealed record ObjectUploadIntent(
    ObjectAddress Address,
    string ContentType,
    long ContentLength,
    ObjectUploadMode Mode,
    long PartSize);

public sealed record ObjectUploadSession(string? ProviderUploadId);

public sealed record SignedObjectRequest(
    Uri Url,
    DateTimeOffset ExpiresAt,
    IReadOnlyDictionary<string, string> Headers);

public sealed record CompletedObjectPart(int PartNumber, string ETag);

public sealed record ObjectInfo(long ContentLength, string ContentType, string? ETag);

/// <summary>
/// 服务端代理上传的数据面 seam。控制器只提交文件流，具体写入本地磁盘还是 OSS 由 adapter 完成。
/// </summary>
public interface IObjectStorageDataPlane
{
    string ProviderName { get; }

    Task<string> UploadAsync(
        SignedObjectRequest request,
        Stream content,
        string contentType,
        long contentLength,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// 文件服务与云对象存储之间的稳定 seam。
/// 文件数据由客户端通过签名 URL 传给具体 adapter 的数据端点；本接口仅处理控制面操作和小范围内容读取。
/// 新增云厂商时实现此契约，上层业务和文件元数据表无需改变。
/// </summary>
public interface IObjectStorageProvider
{
    string Name { get; }
    string Bucket { get; }

    Task<ObjectUploadSession> InitializeUploadAsync(
        ObjectUploadIntent intent,
        CancellationToken cancellationToken = default);

    Task<SignedObjectRequest> SignUploadAsync(
        ObjectUploadIntent intent,
        string? providerUploadId,
        int? partNumber,
        TimeSpan lifetime,
        CancellationToken cancellationToken = default);

    Task CompleteUploadAsync(
        ObjectUploadIntent intent,
        string providerUploadId,
        IReadOnlyList<CompletedObjectPart> parts,
        CancellationToken cancellationToken = default);

    Task AbortUploadAsync(
        ObjectAddress address,
        string providerUploadId,
        CancellationToken cancellationToken = default);

    Task<ObjectInfo?> GetInfoAsync(
        ObjectAddress address,
        CancellationToken cancellationToken = default);

    Task<byte[]> ReadRangeAsync(
        ObjectAddress address,
        long offset,
        int length,
        CancellationToken cancellationToken = default);

    Task<SignedObjectRequest> SignDownloadAsync(
        ObjectAddress address,
        string fileName,
        ObjectDownloadDisposition disposition,
        TimeSpan lifetime,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(ObjectAddress address, CancellationToken cancellationToken = default);

    Task CheckHealthAsync(CancellationToken cancellationToken = default);
}

public interface IObjectStorageProviderResolver
{
    IObjectStorageProvider Resolve(string providerName);
}

public sealed class ObjectStorageProviderResolver : IObjectStorageProviderResolver
{
    private readonly IReadOnlyDictionary<string, IObjectStorageProvider> _providers;

    public ObjectStorageProviderResolver(IEnumerable<IObjectStorageProvider> providers)
    {
        var materialized = providers.ToArray();
        _providers = materialized.ToDictionary(provider => provider.Name, StringComparer.OrdinalIgnoreCase);
        if (_providers.Count != materialized.Length)
            throw new InvalidOperationException("Object storage provider names must be unique.");
    }

    public IObjectStorageProvider Resolve(string providerName) =>
        // 必须按文件记录中固化的 Provider 路由；DefaultProvider 只影响新文件。
        _providers.TryGetValue(providerName, out var provider)
            ? provider
            : throw new KeyNotFoundException($"Object storage provider '{providerName}' is not registered.");
}
