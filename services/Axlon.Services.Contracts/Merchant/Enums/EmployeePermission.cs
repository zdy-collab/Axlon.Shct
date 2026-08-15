using System.ComponentModel;

namespace Axlon.Services.Contracts.Merchant.Enums
{
    /// <summary>
    /// 权限枚举（用于权限控制）
    /// </summary>
    public enum EmployeePermission
    {
        [Description("点单")]
        点单 = 1,

        [Description("收银")]
        收银 = 2,

        [Description("退菜")]
        退菜 = 3,

        [Description("退款")]
        退款 = 4,

        [Description("查看报表")]
        查看报表 = 5,

        [Description("管理员工")]
        管理员工 = 6
    }
}
