using Axlon.Services.Contracts.Models;
using SqlSugar;

namespace Axlon.Services.Contracts.User;

[SugarTable("user_footprint_event_receipts", "用户足迹事件回执")]
[SugarIndex("ux_user_footprint_event_receipts_event", nameof(EventId), OrderByType.Asc, true)]
public sealed class UserFootprintEventReceipt : AuditRoot
{
    [SugarColumn(ColumnName = "event_id", Length = 36)]
    public string EventId { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "user_id")]
    public long UserId { get; set; }

    [SugarColumn(ColumnName = "processed_at")]
    public DateTime ProcessedAt { get; set; }
}
