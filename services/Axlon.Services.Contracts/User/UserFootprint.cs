using Axlon.Services.Contracts.Enums;
using Axlon.Services.Contracts.Models;
using SqlSugar;

namespace Axlon.Services.Contracts.User
{
    /// <summary>
    /// 用户足迹
    /// </summary>
    [SugarTable("user_footprints", "用户足迹")]
    [SugarIndex("ux_user_footprints_user_target_type", nameof(UserId), OrderByType.Asc, nameof(TargetKey), OrderByType.Asc, nameof(FootprintType), OrderByType.Asc, true)]
    public sealed class UserFootprint : AuditRoot
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [SugarColumn(ColumnName = "user_id", ColumnDescription = "用户id")]
        public long UserId { get; set; }

        /// <summary>
        /// 商家id
        /// </summary>
        [SugarColumn(ColumnName = "merchant_id", IsNullable = true, ColumnDescription = "商家id")]
        public long? MerchantId { get; set; }

        /// <summary>
        /// 目标类型：merchant/product/group_buy/content/page
        /// </summary>
        [SugarColumn(ColumnName = "target_type", Length = 30)]
        public string TargetType { get; set; } = string.Empty;

        /// <summary>
        /// 业务目标Id；页面足迹为空
        /// </summary>
        [SugarColumn(ColumnName = "target_id", IsNullable = true)]
        public long? TargetId { get; set; }

        /// <summary>
        /// 用于幂等聚合的规范键，例如 merchant:1001、page:wallet
        /// </summary>
        [SugarColumn(ColumnName = "target_key", Length = 200)]
        public string TargetKey { get; set; } = string.Empty;

        [SugarColumn(ColumnName = "page_code", Length = 100, IsNullable = true)]
        public string? PageCode { get; set; }

        [SugarColumn(ColumnName = "target_title", Length = 200, IsNullable = true)]
        public string? TargetTitle { get; set; }

        [SugarColumn(ColumnName = "target_image", Length = 500, IsNullable = true)]
        public string? TargetImage { get; set; }

        /// <summary>
        /// 足迹类型    
        /// </summary>
        [SugarColumn(ColumnName = "footprint_type", Length = 20)]
        public FootprintTypeEnum FootprintType { get; set; }

        /// <summary>
        /// 订单id
        /// </summary>
        [SugarColumn(ColumnName = "order_id", IsNullable = true, ColumnDescription = "订单id")]
        public long? OrderId { get; set; }

        /// <summary>
        /// 浏览次数
        /// </summary>
        [SugarColumn(ColumnName = "occurrence_count", ColumnDescription = "浏览次数")]
        public int OccurrenceCount { get; set; } = 1;

    }
}
