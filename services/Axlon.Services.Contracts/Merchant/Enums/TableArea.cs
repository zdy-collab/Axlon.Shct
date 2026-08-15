using System.ComponentModel;

namespace Axlon.Services.Contracts.Merchant.Enums
{
    /// <summary>
    /// 区域类型枚举（可选）
    /// </summary>
    public enum TableArea
    {
        [Description("大厅")]
        大厅 = 1,

        [Description("包间")]
        包间 = 2,

        [Description("卡座")]
        卡座 = 3,

        [Description("包厢")]
        包厢 = 4
    }
}
