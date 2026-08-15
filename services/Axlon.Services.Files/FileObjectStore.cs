using Axlon.Services.Contracts.Models.Files;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Axlon.Services.Files;

internal interface IFileObjectStore
{
    Task InsertAsync(FileObject file, CancellationToken cancellationToken);
    Task<FileObject?> GetAsync(long id, CancellationToken cancellationToken);
    Task UpdateAsync(FileObject file, CancellationToken cancellationToken);
    Task<IReadOnlyList<FileObject>> FindCleanupCandidatesAsync(DateTime now, int limit, CancellationToken cancellationToken);
    Task<bool> TryLeaseCleanupAsync(long id, DateTime now, DateTime leaseUntil, CancellationToken cancellationToken);
}

internal sealed class SqlFileObjectStore(ISqlSugarClient db) : IFileObjectStore
{
    public async Task InsertAsync(FileObject file, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await db.Insertable(file).ExecuteCommandAsync();
    }

    public async Task<FileObject?> GetAsync(long id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await db.Queryable<FileObject>().Where(file => file.Id == id).SingleAsync();
    }

    public async Task UpdateAsync(FileObject file, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        file.UpdatedAt = DateTime.UtcNow;
        var affected = await db.Updateable(file).WhereColumns(item => item.Id).ExecuteCommandAsync();
        if (affected != 1) throw new InvalidOperationException($"File metadata update affected {affected} rows.");
    }

    public async Task<IReadOnlyList<FileObject>> FindCleanupCandidatesAsync(
        DateTime now,
        int limit,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await db.Queryable<FileObject>()
            .Where(file => file.ObjectPurgedAt == null
                && (file.CleanupLeaseUntil == null || file.CleanupLeaseUntil < now)
                && ((file.Status == FileStatuses.Pending && file.SessionExpiresAt <= now)
                    || file.Status == FileStatuses.Expired
                    || file.Status == FileStatuses.Failed
                    || (file.Status == FileStatuses.Deleted && file.PurgeAfter <= now)))
            .OrderBy(file => file.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<bool> TryLeaseCleanupAsync(
        long id,
        DateTime now,
        DateTime leaseUntil,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        // 抢占和次数递增必须在一条 SQL 内完成，不能依赖进程内锁，否则多副本会重复清理。
        var affected = await db.Ado.ExecuteCommandAsync(
            "UPDATE `file_objects` SET `cleanup_lease_until`=@lease, `cleanup_attempts`=`cleanup_attempts`+1 " +
            "WHERE `Id`=@id AND `object_purged_at` IS NULL " +
            "AND (`cleanup_lease_until` IS NULL OR `cleanup_lease_until`<@now)",
            new SugarParameter("@lease", leaseUntil),
            new SugarParameter("@id", id),
            new SugarParameter("@now", now));
        return affected == 1;
    }
}
