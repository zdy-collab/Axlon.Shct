using Axlon.Services.Contracts.Base.CommonEnum;
using Axlon.Services.Contracts.Merchant.RootTkey;
using SqlSugar;

namespace Axlon.Services.Contracts.Merchant
{
    /// <summary>
    /// 收银员子账号表
    /// </summary>
    [Tenant("Main")]
    [SugarTable("merchant_employees", "收银员子账号表")]
    public class MerchantEmployees : MerchantEmployeesRoot<long>
    {
        /// <summary>
        /// 角色：cashier-收银员 / waiter-服务员
        /// </summary>
        [SugarColumn(ColumnName = "role", ColumnDataType = "varchar(20)", IsNullable = false)]
        public string Role { get; set; }

        /// <summary>
        /// 权限列表（JSON数组，如["点单","收银","退菜","退款"]）,EmployeePermission
        /// </summary>
        [SugarColumn(ColumnName = "permissions", ColumnDataType = "json", IsJson = true, IsNullable = true)]
        public List<string> Permissions { get; set; }

        /// <summary>
        /// 是否同时是推广员：0-否 / 1-是
        /// </summary>
        [SugarColumn(ColumnName = "is_promoter", ColumnDataType = "tinyint", IsNullable = false, DefaultValue = "0")]
        public YesNo IsPromoter { get; set; } = YesNo.否;

        /// <summary>
        /// 状态：1-启用 / 0-停用
        /// </summary>
        [SugarColumn(ColumnName = "status", ColumnDataType = "tinyint", IsNullable = false, DefaultValue = "1")]
        public DisableEnable Status { get; set; } = DisableEnable.启用;
    }
}
