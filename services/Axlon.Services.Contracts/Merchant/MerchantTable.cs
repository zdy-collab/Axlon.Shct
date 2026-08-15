using Axlon.Services.Contracts.Merchant.Enums;
using Axlon.Services.Contracts.Merchant.RootTkey;
using SqlSugar;

namespace Axlon.Services.Contracts.Merchant
{
    /// <summary>
    /// 桌台信息表
    /// </summary>
    [Tenant("Main")]
    [SugarTable("merchant_tables", "桌台信息表")]
    public class MerchantTable : MerchantTableRoot<long>
    {

        /// <summary>
        /// 桌号（如：A01、B02、VIP03）
        /// </summary>
        [SugarColumn(ColumnName = "table_no", ColumnDataType = "varchar(20)", IsNullable = false)]
        public string TableNo { get; set; }

        /// <summary>
        /// 区域（大厅/包间/卡座/包厢）,Enum：TableArea
        /// </summary>
        [SugarColumn(ColumnName = "area", ColumnDataType = "varchar(50)", IsNullable = true)]
        public string Area { get; set; }

        /// <summary>
        /// 人数上限
        /// </summary>
        [SugarColumn(ColumnName = "capacity", IsNullable = false, DefaultValue = "1")]
        public int Capacity { get; set; } = 1;

        /// <summary>
        /// 桌台状态：0-空闲 / 1-已开台 / 2-待结账 / 3-停用
        /// </summary>
        [SugarColumn(ColumnName = "status", ColumnDataType = "tinyint", IsNullable = false, DefaultValue = "0")]
        public TableStatus Status { get; set; } = TableStatus.空闲;
    }
}
