using System.ComponentModel;

namespace Axlon.Services.Contracts.Merchant.Enums
{
    /// <summary>
    /// 员工状态枚举
    /// </summary>
    public enum EmployeeStatus
    {
        [Description("停用")]
        停用 = 0,

        [Description("启用")]
        启用 = 1
    }
}
