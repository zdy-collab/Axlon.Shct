using System.ComponentModel;

namespace Axlon.Services.Contracts.Merchant.Enums
{
    /// <summary>
    /// 打印机状态枚举
    /// </summary>
    public enum PrinterStatus
    {
        /// <summary>停用</summary>
        [Description("停用")]
        停用 = 0,

        [Description("启用")]
        启用 = 1
    }
}
