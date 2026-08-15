using Axlon.Services.Contracts.Merchant.RootTkey;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axlon.Services.Contracts.Merchant
{
    [Tenant("Main")]
    [SugarTable("reservations", "预约记录")]
    public class Reservation: ReservationRoot<long>
    {
        /// <summary>
        /// 预约日期（仅日期，不含时间）
        /// </summary>
        [SugarColumn(ColumnName = "reservation_date", ColumnDataType = "date", IsNullable = false, ColumnDescription = "预约日期")]
        public DateTime ReservationDate { get; set; }

        /// <summary>
        /// 预约时段，如 "11:00-13:00"
        /// </summary>
        [SugarColumn(ColumnName = "reservation_time", Length = 20, IsNullable = false, ColumnDescription = "预约时段")]
        public string ReservationTime { get; set; }

        /// <summary>
        /// 用餐人数
        /// </summary>
        [SugarColumn(ColumnName = "guest_count", ColumnDataType = "int", IsNullable = false, ColumnDescription = "人数")]
        public int GuestCount { get; set; }

        /// <summary>
        /// 桌型要求，如 "大厅"、"包间"
        /// </summary>
        [SugarColumn(ColumnName = "table_type", ColumnDataType = "varchar(50)", IsNullable = false, ColumnDescription = "桌型要求")]
        public string TableType { get; set; }

        /// <summary>
        /// 联系人姓名
        /// </summary>
        [SugarColumn(ColumnName = "contact_name", ColumnDataType = "varchar(50)", IsNullable = false, ColumnDescription = "联系人")]
        public string ContactName { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [SugarColumn(ColumnName = "contact_phone", ColumnDataType = "varchar(20)", IsNullable = false, ColumnDescription = "联系电话")]
        public string ContactPhone { get; set; }

        /// <summary>
        /// 预约状态
        /// pending-待确认 confirmed-已确认 rejected-已拒绝 cancelled-已取消 completed-已完成
        /// ReservationStatus
        /// </summary>
        [SugarColumn(ColumnName = "status", ColumnDataType = "varchar(20)", IsNullable = false, DefaultValue = "pending", ColumnDescription = "预约状态：pending/confirmed/rejected/cancelled/completed")]
        public string Status { get; set; }

        /// <summary>
        /// 商家确认时间（确认/拒绝时记录）
        /// </summary>
        [SugarColumn(ColumnName = "merchant_confirm_time", IsNullable = true, ColumnDescription = "商家确认时间")]
        public DateTime? MerchantConfirmTime { get; set; }

        /// <summary>
        /// 取消原因
        /// </summary>
        [SugarColumn(ColumnName = "cancel_reason", IsNullable = true, ColumnDescription = "取消原因")]
        public string? CancelReason { get; set; }
    }
}
