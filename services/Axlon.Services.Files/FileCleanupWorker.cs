using Axlon.Services.Contracts.Models.Files;
using Axlon.Services.Files.ObjectStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
namespace Axlon.Services.Files;

internal sealed class FileCleanupWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<FilesOptions> options,
    TimeProvider timeProvider,
    ILogger<FileCleanupWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await RunBatchSafelyAsync(stoppingToken);
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(options.Value.CleanupIntervalMinutes), timeProvider);
        while (await timer.WaitForNextTickAsync(stoppingToken))
            await RunBatchSafelyAsync(stoppingToken);
    }

    private async Task RunBatchSafelyAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var store = scope.ServiceProvider.GetRequiredService<IFileObjectStore>();
            var providers = scope.ServiceProvider.GetRequiredService<IObjectStorageProviderResolver>();
            var now = timeProvider.GetUtcNow().UtcDateTime;
            var candidates = await store.FindCleanupCandidatesAsync(now, options.Value.CleanupBatchSize, cancellationToken);
            foreach (var candidate in candidates)
            {
                var leaseUntil = now.AddMinutes(options.Value.CleanupLeaseMinutes);
                // 条件更新抢占短租约，多副本只会有一个实例处理同一文件；租约过期后可安全重试。
                if (!await store.TryLeaseCleanupAsync(candidate.Id, now, leaseUntil, cancellationToken)) continue;
                candidate.CleanupLeaseUntil = leaseUntil;
                candidate.CleanupAttempts++;
                await PurgeAsync(candidate, store, providers, cancellationToken);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { }
        catch (Exception exception)
        {
            logger.LogError(exception, "File cleanup batch failed.");
            FilesTelemetry.CleanupFailures.Add(1);
        }
    }

    private async Task PurgeAsync(
        FileObject file,
        IFileObjectStore store,
        IObjectStorageProviderResolver providers,
        CancellationToken cancellationToken)
    {
        var address = new ObjectAddress(file.Provider, file.Bucket, file.ObjectKey);
        try
        {
            var provider = providers.Resolve(file.Provider);
            if (file.ProviderUploadId is not null && file.Status is FileStatuses.Pending or FileStatuses.Expired)
            {
                // Abort 清理未合并的分片，Delete 同时兜底处理“已合并但元数据未更新”的对象。
                try { await provider.AbortUploadAsync(address, file.ProviderUploadId, cancellationToken); }
                catch (Exception exception) { logger.LogInformation(exception, "Multipart upload {FileId} was already absent or could not be aborted.", file.Id); }
            }

            await provider.DeleteAsync(address, cancellationToken);
            var now = timeProvider.GetUtcNow().UtcDateTime;
            file.Status = file.Status switch
            {
                FileStatuses.Deleted => FileStatuses.Purged,
                FileStatuses.Pending => FileStatuses.Expired,
                _ => file.Status
            };
            file.ObjectPurgedAt = now;
            file.CleanupLeaseUntil = null;
            file.LastCleanupError = null;
            file.UpdatedAt = now;
            await store.UpdateAsync(file, cancellationToken);
            FilesTelemetry.ObjectsPurged.Add(1, new KeyValuePair<string, object?>("provider", file.Provider));
        }
        catch (Exception exception)
        {
            file.CleanupLeaseUntil = null;
            file.LastCleanupError = exception.Message.Length <= 1000 ? exception.Message : exception.Message[..1000];
            file.UpdatedAt = timeProvider.GetUtcNow().UtcDateTime;
            try { await store.UpdateAsync(file, cancellationToken); }
            catch (Exception updateException) { logger.LogError(updateException, "Unable to release cleanup lease for {FileId}", file.Id); }
            logger.LogWarning(exception, "Unable to purge object for {FileId} from {Provider}", file.Id, file.Provider);
            FilesTelemetry.CleanupFailures.Add(1, new KeyValuePair<string, object?>("provider", file.Provider));
        }
    }
}
