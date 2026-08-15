using System.ComponentModel;

namespace Axlon.Services.Contracts.Merchant.Enums
{
    /// <summary>
    /// 打印机类型枚举
    /// </summary>
    public enum PrinterTypeEnum
    {
        [Description("蓝牙打印机")]
        Bluetooth = 1,

        [Description("云打印机")]
        Cloud = 2
    }
}
