using Axlon.Services.Contracts.GroupBuy.Enums;
using Axlon.Services.Contracts.GroupBuy.JsonObj;
using Axlon.Services.Contracts.GroupBuy.RootTkey;
using SqlSugar;

namespace Axlon.Services.Contracts.GroupBuy
{
    /// <summary>
    /// 团购商品表
    /// </summary>
    [Tenant("Main")]
    [SugarTable("group_buys", "团购商品表")]
    public class GroupBuys : GroupBuysRoot<long>
    {
        /// <summary>
        /// 团购标题
        /// </summary>
        [SugarColumn(ColumnName = "title", ColumnDescription = "团购标题", Length = 200, IsNullable = false)]
        public string Title { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        [SugarColumn(ColumnName = "image_oss", IsNullable = true)]
        public string ImageOss { get; set; }

        /// <summary>
        /// 包含菜品ID及数量，JSON格式：[{"product_id":1,"count":2}]
        /// </summary>
        [SugarColumn(ColumnName = "product_ids", ColumnDescription = "包含菜品ID及数量(JSON格式)", IsJson = true,
            IsNullable = false)]
        public ProductIds ProductIds { get; set; }

        /// <summary>
        /// 原价
        /// </summary>
        [SugarColumn(ColumnName = "original_price", ColumnDescription = "原价", DecimalDigits = 2,
            IsNullable = false)]
        public decimal OriginalPrice { get; set; }

        /// <summary>
        /// 团购价
        /// </summary>
        [SugarColumn(ColumnName = "group_price", ColumnDescription = "团购价", DecimalDigits = 2,
            IsNullable = false)]
        public decimal GroupPrice { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        [SugarColumn(ColumnName = "stock", ColumnDescription = "库存", IsNullable = false)]
        public int Stock { get; set; }

        /// <summary>
        /// 已售数量
        /// </summary>
        [SugarColumn(ColumnName = "sold_count", ColumnDescription = "已售数量", IsNullable = false, DefaultValue = "0")]
        public int SoldCount { get; set; }

        /// <summary>
        /// 团购类型：gift-赠品型，subsidy-补贴型，GroupBuysType
        /// </summary>
        [SugarColumn(ColumnName = "type", ColumnDescription = "团购类型(gift-赠品型/subsidy-补贴型)", Length = 20,
            IsNullable = false)]
        public string Type { get; set; }

        /// <summary>
        /// 使用规则，JSON格式
        /// </summary>
        [SugarColumn(ColumnName = "use_rules", ColumnDescription = "使用规则(JSON格式)", IsJson = true, IsNullable = true)]
        public UseRules UseRules { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [SugarColumn(ColumnName = "start_time", ColumnDescription = "开始时间", IsNullable = false)]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [SugarColumn(ColumnName = "end_time", ColumnDescription = "结束时间", IsNullable = false)]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 是否上架：1-上架，0-下架
        /// </summary>
        [SugarColumn(ColumnName = "is_on", ColumnDataType = "tinyint", ColumnDescription = "是否上架(1上架/0下架)",
            IsNullable = false)]
        public IsOn IsOn { get; set; }
    }
}
