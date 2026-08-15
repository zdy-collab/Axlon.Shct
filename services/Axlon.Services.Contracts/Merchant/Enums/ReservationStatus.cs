using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Axlon.Services.Contracts.Merchant.Enums
{
    /// <summary>
    /// 预约状态
    /// </summary>
    public enum ReservationStatus
    {
        [Description("待确认")]
        pending,

        [Description("已确认")]
        confirmed,
        
        [Description("已拒绝")]
        rejected,

        [Description("已取消")]
        cancelled,

        [Description("已完成")]
        completed
    }
}
