using System.ComponentModel;

namespace Axlon.Services.Contracts.Merchant.Enums
{
    /// <summary>
    /// 员工角色枚举
    /// </summary>
    public enum EmployeeRole
    {
        [Description("收银员")]
        Cashier = 1,

        [Description("服务员")]
        Waiter = 2
    }
}
