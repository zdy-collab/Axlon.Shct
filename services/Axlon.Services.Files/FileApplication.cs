using Axlon.Framework.Core.DependencyInjection;
using Axlon.Framework.Core.HttpContextUser;
using Axlon.Services.Contracts.Models.Files;
using Axlon.Services.Files.ObjectStorage;
using Axlon.Services.Files.OutInput;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axlon.Services.Files;

internal interface IFileApplication
{
    Task<BeginFileUploadOutput> BeginUploadAsync(
        BeginFileUploadRequest request,
        CancellationToken cancellationToken,
        string? providerName = null);
    Task<SignedRequestOutput> RenewUploadUrlAsync(long id, CancellationToken cancellationToken);
    Task<IReadOnlyList<SignedFilePartOutput>> SignPartsAsync(long id, SignFilePartsRequest request, CancellationToken cancellationToken);
    Task<FileMetadataOutput> CompleteUploadAsync(long id, CompleteFileUploadRequest request, CancellationToken cancellationToken);
    Task AbortUploadAsync(long id, CancellationToken cancellationToken);
    Task<FileMetadataOutput> GetAsync(long id, CancellationToken cancellationToken);
    Task<DownloadUrlOutput> CreateDownloadUrlAsync(
        long id,
        CreateDownloadUrlRequest request,
        CancellationToken cancellationToken,
        string? expectedProvider = null);
    Task DeleteAsync(long id, CancellationToken cancellationToken);
}

internal sealed class FileApplication(
    IFileObjectStore store,
    IObjectStorageProviderResolver providers,
    IUser user,
    IAxlonIdGenerator idGenerator,
    FilesOptions options,
    TimeProvider timeProvider,
    ILogger<FileApplication> logger) : IFileApplication
{
    public async Task<BeginFileUploadOutput> BeginUploadAsync(
        BeginFileUploadRequest request,
        CancellationToken cancellationToken,
        string? providerName = null)
    {
        EnsureAuthenticated();
        var media = new FileMediaPolicy(options).ValidateDeclaration(request.FileName, request.ContentType, request.Size);
        var visibility = request.Visibility?.Trim().ToLowerInvariant() ?? string.Empty;
        if (!FileVisibilities.IsValid(visibility))
            throw FileServiceException.BadRequest("Visibility must be private or tenant.");

        var now = UtcNow;
        var provider = Resolve(providerName ?? options.DefaultProvider);
        var id = idGenerator.NextId();
        var mode = request.Size > options.MultipartThresholdBytes
            ? ObjectUploadMode.Multipart
            : ObjectUploadMode.Single;
        // object key 完全由可信的租户、时间和服务端 ID 构造，原文件名只用于展示，避免路径穿越和覆盖。
        var objectKey = $"tenants/{user.TenantId}/{now:yyyy/MM/dd}/{id}{media.Extension}";
        var address = new ObjectAddress(provider.Name, provider.Bucket, objectKey);
        var intent = new ObjectUploadIntent(address, media.ContentType, request.Size, mode, options.PartSizeBytes);

        ObjectUploadSession session;
        try
        {
            session = await provider.InitializeUploadAsync(intent, cancellationToken);
        }
        catch (Exception exception)
        {
            throw FileServiceException.BadGateway("Unable to initialize the object upload.", exception);
        }

        var file = new FileObject
        {
            Id = id,
            TenantId = user.TenantId,
            OwnerUserId = user.ID,
            OriginalName = media.DisplayName,
            Extension = media.Extension,
            MediaKind = media.MediaKind,
            ContentType = media.ContentType,
            DeclaredSize = request.Size,
            // Provider/Bucket/ObjectKey 随文件固化；以后切换默认云厂商也不会影响历史文件读取。
            Provider = provider.Name,
            Bucket = provider.Bucket,
            ObjectKey = objectKey,
            ProviderUploadId = session.ProviderUploadId,
            Visibility = visibility,
            Status = FileStatuses.Pending,
            UploadMode = mode == ObjectUploadMode.Multipart ? FileUploadModes.Multipart : FileUploadModes.Single,
            PartSize = mode == ObjectUploadMode.Multipart ? options.PartSizeBytes : null,
            SessionExpiresAt = now.AddHours(options.UploadSessionLifetimeHours),
            CreatedAt = now,
            UpdatedAt = now
        };

        try
        {
            await store.InsertAsync(file, cancellationToken);
        }
        catch
        {
            if (mode == ObjectUploadMode.Multipart && session.ProviderUploadId is not null)
            {
                try { await provider.AbortUploadAsync(address, session.ProviderUploadId, cancellationToken); }
                catch (Exception exception) { logger.LogWarning(exception, "Failed to abort orphan upload for {FileId}", id); }
            }
            throw;
        }

        SignedRequestOutput? signed = null;
        if (mode == ObjectUploadMode.Single)
            signed = ToOutput(await SignAsync(provider, intent, null, null, cancellationToken));

        FilesTelemetry.SessionsCreated.Add(1, new KeyValuePair<string, object?>("provider", provider.Name));
        return new BeginFileUploadOutput(
            id,
            file.UploadMode,
            file.PartSize,
            mode == ObjectUploadMode.Multipart ? GetPartCount(file) : null,
            file.SessionExpiresAt,
            signed);
    }

    public async Task<SignedRequestOutput> RenewUploadUrlAsync(long id, CancellationToken cancellationToken)
    {
        var file = await GetOwnedPendingAsync(id, cancellationToken);
        if (file.UploadMode != FileUploadModes.Single)
            throw FileServiceException.Conflict("Multipart uploads use the part URL endpoint.");

        var provider = Resolve(file.Provider);
        return ToOutput(await SignAsync(provider, ToIntent(file), null, null, cancellationToken));
    }

    public async Task<IReadOnlyList<SignedFilePartOutput>> SignPartsAsync(
        long id,
        SignFilePartsRequest request,
        CancellationToken cancellationToken)
    {
        var file = await GetOwnedPendingAsync(id, cancellationToken);
        if (file.UploadMode != FileUploadModes.Multipart || string.IsNullOrWhiteSpace(file.ProviderUploadId))
            throw FileServiceException.Conflict("This file does not have a multipart upload session.");

        var requestedNumbers = request.PartNumbers ?? [];
        var numbers = requestedNumbers.Distinct().Order().ToArray();
        if (numbers.Length == 0 || numbers.Length != requestedNumbers.Length || numbers.Length > 50)
            throw FileServiceException.BadRequest("Part numbers must contain 1 to 50 unique values.");

        var count = GetPartCount(file);
        if (numbers.Any(number => number < 1 || number > count))
            throw FileServiceException.BadRequest($"Part numbers must be between 1 and {count}.");

        var provider = Resolve(file.Provider);
        var results = new List<SignedFilePartOutput>(numbers.Length);
        foreach (var number in numbers)
        {
            var partLength = GetPartLength(file, number);
            var intent = ToIntent(file) with { ContentLength = partLength };
            var signed = await SignAsync(provider, intent, file.ProviderUploadId, number, cancellationToken);
            results.Add(new SignedFilePartOutput(number, ToOutput(signed)));
        }
        return results;
    }

    public async Task<FileMetadataOutput> CompleteUploadAsync(
        long id,
        CompleteFileUploadRequest request,
        CancellationToken cancellationToken)
    {
        EnsureAuthenticated();
        var file = await store.GetAsync(id, cancellationToken) ?? throw FileServiceException.NotFound();
        EnsureOwner(file);
        if (file.Status == FileStatuses.Ready) return ToMetadata(file);
        if (file.Status != FileStatuses.Pending)
            throw FileServiceException.Conflict($"Upload cannot be completed from status '{file.Status}'.");
        EnsureSessionActive(file);

        var provider = Resolve(file.Provider);
        var intent = ToIntent(file);
        if (file.UploadMode == FileUploadModes.Multipart)
        {
            var parts = ValidateCompletionParts(file, request.Parts ?? []);
            try
            {
                await provider.CompleteUploadAsync(intent, file.ProviderUploadId!, parts, cancellationToken);
            }
            catch (Exception exception)
            {
                // OSS 可能已经合并成功，但响应或数据库更新失败；HEAD 能恢复这一幂等完成场景。
                var recovered = await TryGetInfoAsync(provider, intent.Address, cancellationToken);
                if (recovered is null)
                    throw FileServiceException.BadGateway("Unable to complete the multipart upload.", exception);
            }
        }
        else if ((request.Parts?.Length ?? 0) != 0)
        {
            throw FileServiceException.BadRequest("Single uploads do not accept multipart ETags.");
        }

        ObjectInfo info;
        try
        {
            info = await provider.GetInfoAsync(intent.Address, cancellationToken)
                ?? throw FileServiceException.Conflict("The uploaded object does not exist.");
            // 仅拉取前 4 KiB 做文件签名校验，视频和图片主体始终不经过应用服务器。
            var prefix = await provider.ReadRangeAsync(intent.Address, 0, 4096, cancellationToken);
            new FileMediaPolicy(options).ValidateStoredObject(file, info.ContentLength, info.ContentType, prefix);
        }
        catch (FileServiceException exception) when (exception.StatusCode == 415)
        {
            try { await provider.DeleteAsync(intent.Address, cancellationToken); }
            catch (Exception deleteException) { logger.LogWarning(deleteException, "Failed to delete rejected object {FileId}", id); }
            file.Status = FileStatuses.Failed;
            file.UpdatedAt = UtcNow;
            await store.UpdateAsync(file, cancellationToken);
            FilesTelemetry.UploadsFailed.Add(1);
            throw;
        }
        catch (FileServiceException) { throw; }
        catch (Exception exception)
        {
            throw FileServiceException.BadGateway("Unable to verify the uploaded object.", exception);
        }

        file.ActualSize = info.ContentLength;
        file.ProviderETag = info.ETag;
        file.Status = FileStatuses.Ready;
        file.ReadyAt = UtcNow;
        file.UpdatedAt = UtcNow;
        await store.UpdateAsync(file, cancellationToken);
        FilesTelemetry.UploadsCompleted.Add(1, new KeyValuePair<string, object?>("provider", provider.Name));
        return ToMetadata(file);
    }

    public async Task AbortUploadAsync(long id, CancellationToken cancellationToken)
    {
        var file = await GetOwnedPendingAsync(id, cancellationToken, requireActiveSession: false);
        var provider = Resolve(file.Provider);
        try
        {
            if (file.UploadMode == FileUploadModes.Multipart && file.ProviderUploadId is not null)
            {
                try
                {
                    await provider.AbortUploadAsync(ToAddress(file), file.ProviderUploadId, cancellationToken);
                }
                catch when (!cancellationToken.IsCancellationRequested)
                {
                    // 分片会话可能已完成或已终止；继续删除最终对象，形成可重复调用的清理边界。
                }
            }
            await provider.DeleteAsync(ToAddress(file), cancellationToken);
        }
        catch (Exception exception)
        {
            throw FileServiceException.BadGateway("Unable to abort the upload.", exception);
        }

        file.Status = FileStatuses.Expired;
        file.ObjectPurgedAt = UtcNow;
        file.UpdatedAt = UtcNow;
        await store.UpdateAsync(file, cancellationToken);
    }

    public async Task<FileMetadataOutput> GetAsync(long id, CancellationToken cancellationToken)
    {
        var file = await GetReadableReadyAsync(id, cancellationToken);
        return ToMetadata(file);
    }

    public async Task<DownloadUrlOutput> CreateDownloadUrlAsync(
        long id,
        CreateDownloadUrlRequest request,
        CancellationToken cancellationToken,
        string? expectedProvider = null)
    {
        var file = await GetReadableReadyAsync(id, cancellationToken);
        if (!string.IsNullOrWhiteSpace(expectedProvider)
            && !file.Provider.Equals(expectedProvider, StringComparison.OrdinalIgnoreCase))
            throw FileServiceException.NotFound();
        var disposition = request.Disposition?.Trim().ToLowerInvariant() switch
        {
            "inline" => ObjectDownloadDisposition.Inline,
            "attachment" => ObjectDownloadDisposition.Attachment,
            _ => throw FileServiceException.BadRequest("Disposition must be inline or attachment.")
        };

        try
        {
            var signed = await Resolve(file.Provider).SignDownloadAsync(
                ToAddress(file), file.OriginalName, disposition,
                TimeSpan.FromMinutes(options.SignedUrlLifetimeMinutes), cancellationToken);
            FilesTelemetry.UrlsSigned.Add(1, new KeyValuePair<string, object?>("operation", "download"));
            return new DownloadUrlOutput(signed.Url.ToString(), signed.ExpiresAt.UtcDateTime,
                file.OriginalName, file.ContentType);
        }
        catch (Exception exception)
        {
            throw FileServiceException.BadGateway("Unable to authorize the download.", exception);
        }
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken)
    {
        EnsureAuthenticated();
        var file = await store.GetAsync(id, cancellationToken) ?? throw FileServiceException.NotFound();
        EnsureOwner(file);
        if (file.Status is FileStatuses.Deleted or FileStatuses.Purged) return;
        if (file.Status != FileStatuses.Ready)
            throw FileServiceException.Conflict("Only ready files can be deleted. Abort pending uploads instead.");

        var now = UtcNow;
        // 先更新元数据立即阻断下载，云端二进制由后台任务在保留期后删除。
        file.Status = FileStatuses.Deleted;
        file.DeletedAt = now;
        file.PurgeAfter = now.AddDays(options.DeletedRetentionDays);
        file.UpdatedAt = now;
        await store.UpdateAsync(file, cancellationToken);
    }

    private async Task<FileObject> GetOwnedPendingAsync(
        long id,
        CancellationToken cancellationToken,
        bool requireActiveSession = true)
    {
        EnsureAuthenticated();
        var file = await store.GetAsync(id, cancellationToken) ?? throw FileServiceException.NotFound();
        EnsureOwner(file);
        if (file.Status != FileStatuses.Pending)
            throw FileServiceException.Conflict($"Upload is in status '{file.Status}'.");
        if (requireActiveSession) EnsureSessionActive(file);
        return file;
    }

    private async Task<FileObject> GetReadableReadyAsync(long id, CancellationToken cancellationToken)
    {
        var file = await store.GetAsync(id, cancellationToken) ?? throw FileServiceException.NotFound();
        if (file.Status != FileStatuses.Ready) throw FileServiceException.NotFound();
        // FileVisibilities.Public 不进行鉴权
        if (file.Visibility != FileVisibilities.Public) 
        {
            EnsureAuthenticated();
            // 即使 visibility=tenant，也绝不允许跨租户读取；private 只允许上传者本人。
            var sameTenant = file.TenantId == user.TenantId;
            var readable = file.OwnerUserId == user.ID
                || (file.Visibility == FileVisibilities.Tenant && sameTenant);
            if (!readable || !sameTenant) throw FileServiceException.Forbidden();
        }

        return file;
    }

    private void EnsureAuthenticated()
    {
        if (!user.IsAuthenticated() || user.ID <= 0)
            throw new FileServiceException(401, "unauthorized", "Authentication is required.");
    }

    private void EnsureOwner(FileObject file)
    {
        if (file.OwnerUserId != user.ID || file.TenantId != user.TenantId)
            throw FileServiceException.Forbidden();
    }

    private void EnsureSessionActive(FileObject file)
    {
        if (file.SessionExpiresAt <= UtcNow)
            throw FileServiceException.Gone("The upload session has expired.");
    }

    private IObjectStorageProvider Resolve(string name)
    {
        try { return providers.Resolve(name); }
        catch (Exception exception) { throw FileServiceException.StorageUnavailable($"Storage provider '{name}' is unavailable.", exception); }
    }

    private async Task<SignedObjectRequest> SignAsync(
        IObjectStorageProvider provider,
        ObjectUploadIntent intent,
        string? uploadId,
        int? partNumber,
        CancellationToken cancellationToken)
    {
        try
        {
            var signed = await provider.SignUploadAsync(intent, uploadId, partNumber,
                TimeSpan.FromMinutes(options.SignedUrlLifetimeMinutes), cancellationToken);
            FilesTelemetry.UrlsSigned.Add(1, new KeyValuePair<string, object?>("operation", "upload"));
            return signed;
        }
        catch (Exception exception)
        {
            throw FileServiceException.BadGateway("Unable to sign the upload request.", exception);
        }
    }

    private static IReadOnlyList<CompletedObjectPart> ValidateCompletionParts(
        FileObject file,
        IReadOnlyList<CompletedFilePartInput> input)
    {
        var expected = GetPartCount(file);
        if (input.Count != expected)
            throw FileServiceException.BadRequest($"Multipart completion requires exactly {expected} parts.");
        var ordered = input.OrderBy(part => part.PartNumber).ToArray();
        for (var index = 0; index < ordered.Length; index++)
        {
            if (ordered[index].PartNumber != index + 1 || string.IsNullOrWhiteSpace(ordered[index].ETag))
                throw FileServiceException.BadRequest("Multipart parts must be unique, contiguous and include ETags.");
        }
        return ordered.Select(part => new CompletedObjectPart(part.PartNumber, part.ETag)).ToArray();
    }

    private static int GetPartCount(FileObject file) =>
        checked((int)((file.DeclaredSize + file.PartSize!.Value - 1) / file.PartSize.Value));

    private static long GetPartLength(FileObject file, int partNumber)
    {
        var partSize = file.PartSize!.Value;
        var offset = checked((partNumber - 1L) * partSize);
        return Math.Min(partSize, file.DeclaredSize - offset);
    }

    private static ObjectAddress ToAddress(FileObject file) =>
        new(file.Provider, file.Bucket, file.ObjectKey);

    private static ObjectUploadIntent ToIntent(FileObject file) =>
        new(ToAddress(file), file.ContentType, file.DeclaredSize,
            file.UploadMode == FileUploadModes.Multipart ? ObjectUploadMode.Multipart : ObjectUploadMode.Single,
            file.PartSize ?? 0);

    private static SignedRequestOutput ToOutput(SignedObjectRequest request) =>
        new(request.Url.ToString(), request.ExpiresAt.UtcDateTime, request.Headers);

    internal static FileMetadataOutput ToMetadata(FileObject file) =>
        new(file.Id, file.OriginalName, file.MediaKind, file.ContentType,
            file.ActualSize ?? file.DeclaredSize, file.Visibility, file.Status, file.CreatedAt, file.ReadyAt);

    private static async Task<ObjectInfo?> TryGetInfoAsync(
        IObjectStorageProvider provider,
        ObjectAddress address,
        CancellationToken cancellationToken)
    {
        try { return await provider.GetInfoAsync(address, cancellationToken); }
        catch { return null; }
    }

    private DateTime UtcNow => timeProvider.GetUtcNow().UtcDateTime;
}
