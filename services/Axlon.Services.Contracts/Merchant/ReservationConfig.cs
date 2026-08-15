using Axlon.Services.Contracts.Base.CommonEnum;
using Axlon.Services.Contracts.Merchant.RootTkey;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axlon.Services.Contracts.Merchant
{
    [Tenant("Main")]
    [SugarTable("reservation_configs", "商家预约配置")]
    public class ReservationConfig: ReservationConfigRoot<long>
    {
        /// <summary>
        /// 是否开启预约功能 0-否 1-是
        /// </summary>
        [SugarColumn(ColumnName = "is_open", ColumnDataType = "tinyint", IsNullable = false, 
            DefaultValue = "0", ColumnDescription = "是否开启预约")]
        public YesNo IsOpen { get; set; }

        /// <summary>
        /// 可提前预约的天数
        /// 例如：7 表示顾客最多可提前 7 天预约
        /// </summary>
        [SugarColumn(ColumnName = "advance_days", ColumnDataType = "int", IsNullable = false, DefaultValue = "0", ColumnDescription = "可提前预约天数")]
        public int AdvanceDays { get; set; }

        /// <summary>
        /// 可预约时段，JSON 数组格式
        /// 示例：["11:00-13:00", "17:00-19:00"]
        /// </summary>
        [SugarColumn(ColumnName = "time_slots", ColumnDataType = "json", IsNullable = true, IsJson = true
            , ColumnDescription = "可预约时段")]
        public List<string>? TimeSlots { get; set; }

        /// <summary>
        /// 可预约桌型，JSON 数组格式
        /// 示例：["大厅", "包间"]
        /// </summary>
        [SugarColumn(ColumnName = "table_types", ColumnDataType = "json", IsNullable = true, IsJson = true
            , ColumnDescription = "可预约桌型")]
        public string? TableTypes { get; set; }

        /// <summary>
        /// 每时段最大接待人数
        /// 用于控制单个可预约时段内的客流量上限
        /// </summary>
        [SugarColumn(ColumnName = "max_guest_per_slot", ColumnDataType = "int", IsNullable = false, DefaultValue = "0", ColumnDescription = "每时段最大接待人数")]
        public int MaxGuestPerSlot { get; set; }
    }
}
