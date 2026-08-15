using Axlon.Services.Contracts.GroupBuy.RootTkey;
using SqlSugar;

namespace Axlon.Services.Contracts.GroupBuy
{
    /// <summary>
    /// 特价团购精选表（平台运营）
    /// </summary>
    [Tenant("Main")]
    [SugarTable("featured_group_buys", "特价团购精选（平台运营）")]
    public class FeaturedGroupBuys : FeaturedGroupBuysRoot<long>
    {

        /// <summary>
        /// 排序权重（数值越大越靠前）
        /// </summary>
        [SugarColumn(ColumnName = "sort_weight", ColumnDescription = "排序权重", IsNullable = false)]
        public int SortWeight { get; set; }

        /// <summary>
        /// 展示开始时间
        /// </summary>
        [SugarColumn(ColumnName = "start_time", ColumnDescription = "展示开始时间", IsNullable = false)]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 展示结束时间
        /// </summary>
        [SugarColumn(ColumnName = "end_time", ColumnDescription = "展示结束时间", IsNullable = false)]
        public DateTime EndTime { get; set; }
    }
}
