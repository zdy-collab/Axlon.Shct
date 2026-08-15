using Axlon.Services.Contracts.Merchant.RootTkey;
using SqlSugar;

namespace Axlon.Services.Contracts.Merchant
{
    /// <summary>
    /// 商家品类配置（含分佣比例）
    /// </summary>
    [Tenant("Main")]
    [SugarTable("merchant_category_config", "商家品类配置")]
    public class MerchantCategoryConfig : MerchantCategoryConfigRoot<long>
    {
        /// <summary>
        /// 该品类佣金池比例（如0.07=7%）
        /// </summary>
        [SugarColumn(ColumnDataType = "decimal(5,4)", IsNullable = false, DefaultValue = "0", ColumnName = "commission_rate")]
        public decimal CommissionRate { get; set; }
    }
}
