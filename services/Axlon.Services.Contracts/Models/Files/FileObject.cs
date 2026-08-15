using Axlon.Framework.Abstractions;
using SqlSugar;

namespace Axlon.Services.Contracts.Models.Files;

[Tenant("Files")]
[SugarTable("file_objects", "文件对象表")]
[SugarIndex("ux_file_objects_storage", nameof(Provider), OrderByType.Asc, nameof(Bucket), OrderByType.Asc,
    nameof(ObjectKey), OrderByType.Asc, true)]
[SugarIndex("ix_file_objects_tenant_owner", nameof(TenantId), OrderByType.Asc,
    nameof(OwnerUserId), OrderByType.Asc)]
[SugarIndex("ix_file_objects_status_expiry", nameof(Status), OrderByType.Asc,
    nameof(SessionExpiresAt), OrderByType.Asc)]
[SugarIndex("ix_file_objects_purge", nameof(Status), OrderByType.Asc,
    nameof(PurgeAfter), OrderByType.Asc)]
public sealed class FileObject : RootEntityTkey<long>
{
    /// <summary>
    /// 租户Id
    /// </summary>
    [SugarColumn(ColumnName = "tenant_id")]
    public long TenantId { get; set; }

    /// <summary>
    /// 文件所属用户
    /// </summary>
    [SugarColumn(ColumnName = "owner_user_id")]
    public long OwnerUserId { get; set; }

    /// <summary>
    /// 文件名
    /// </summary>
    [SugarColumn(ColumnName = "original_name", Length = 255)]
    public string OriginalName { get; set; } = string.Empty;

    /// <summary>
    /// 文件扩展名
    /// </summary>
    [SugarColumn(ColumnName = "extension", Length = 16)]
    public string Extension { get; set; } = string.Empty;

    /// <summary>
    /// 媒体类型
    /// </summary>
    [SugarColumn(ColumnName = "media_kind", Length = 16)]
    public string MediaKind { get; set; } = string.Empty;

    /// <summary>
    /// HTTP MIME 类型
    /// </summary>
    [SugarColumn(ColumnName = "content_type", Length = 128)]
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// 文件大小
    /// </summary>
    [SugarColumn(ColumnName = "declared_size")]
    public long DeclaredSize { get; set; }

    /// <summary>
    /// 实际大小
    /// </summary>
    [SugarColumn(ColumnName = "actual_size", IsNullable = true)]
    public long? ActualSize { get; set; }

    /// <summary>
    /// 存储提供商
    /// </summary>
    [SugarColumn(ColumnName = "provider", Length = 32)]
    public string Provider { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "bucket", Length = 128)]
    public string Bucket { get; set; } = string.Empty;

    /// <summary>
    /// 对象路径
    /// </summary>
    [SugarColumn(ColumnName = "object_key", Length = 512)]
    public string ObjectKey { get; set; } = string.Empty;

    /// <summary>
    /// 云存储上传任务ID
    /// </summary>
    [SugarColumn(ColumnName = "provider_upload_id", Length = 512, IsNullable = true)]
    public string? ProviderUploadId { get; set; }

    /// <summary>
    /// 云存储返回的文件指纹
    /// </summary>
    [SugarColumn(ColumnName = "provider_etag", Length = 256, IsNullable = true)]
    public string? ProviderETag { get; set; }

    /// <summary>
    /// 文件可见范围
    /// </summary>
    [SugarColumn(ColumnName = "visibility", Length = 16)]
    public string Visibility { get; set; } = FileVisibilities.Private;

    /// <summary>
    /// 文件状态
    /// </summary>
    [SugarColumn(ColumnName = "status", Length = 16)]
    public string Status { get; set; } = FileStatuses.Pending;

    /// <summary>
    /// 上传模式
    /// </summary>
    [SugarColumn(ColumnName = "upload_mode", Length = 16)]
    public string UploadMode { get; set; } = FileUploadModes.Single;

    [SugarColumn(ColumnName = "part_size", IsNullable = true)]
    public long? PartSize { get; set; }

    [SugarColumn(ColumnName = "session_expires_at")]
    public DateTime SessionExpiresAt { get; set; }

    [SugarColumn(ColumnName = "ready_at", IsNullable = true)]
    public DateTime? ReadyAt { get; set; }

    [SugarColumn(ColumnName = "deleted_at", IsNullable = true)]
    public DateTime? DeletedAt { get; set; }

    [SugarColumn(ColumnName = "purge_after", IsNullable = true)]
    public DateTime? PurgeAfter { get; set; }

    [SugarColumn(ColumnName = "object_purged_at", IsNullable = true)]
    public DateTime? ObjectPurgedAt { get; set; }

    [SugarColumn(ColumnName = "cleanup_lease_until", IsNullable = true)]
    public DateTime? CleanupLeaseUntil { get; set; }

    [SugarColumn(ColumnName = "cleanup_attempts")]
    public int CleanupAttempts { get; set; }

    [SugarColumn(ColumnName = "last_cleanup_error", Length = 1000, IsNullable = true)]
    public string? LastCleanupError { get; set; }

    [SugarColumn(ColumnName = "created_at")]
    public DateTime CreatedAt { get; set; }

    [SugarColumn(ColumnName = "updated_at")]
    public DateTime UpdatedAt { get; set; }
}

public static class FileStatuses
{
    public const string Pending = "pending";
    public const string Ready = "ready";
    public const string Deleted = "deleted";
    public const string Purged = "purged";
    public const string Failed = "failed";
    public const string Expired = "expired";
}

public static class FileVisibilities
{
    public const string Private = "private";
    public const string Tenant = "tenant";
    public const string Public = "public";

    public static bool IsValid(string? value) => value is Private or Tenant or Public;
}

public static class FileMediaKinds
{
    public const string Image = "image";
    public const string Video = "video";
}

public static class FileUploadModes
{
    public const string Single = "single";
    public const string Multipart = "multipart";
}
