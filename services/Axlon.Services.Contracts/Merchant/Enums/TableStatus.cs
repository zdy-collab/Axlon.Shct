using System.ComponentModel;

namespace Axlon.Services.Contracts.Merchant.Enums
{
    /// <summary>
    /// 桌台状态枚举
    /// </summary>
    public enum TableStatus
    {
        [Description("空闲")]
        空闲 = 0,

        [Description("已开台")]
        已开台 = 1,

        [Description("待结账")]
        待结账 = 2,

        [Description("停用")]
        停用 = 3
    }
}
