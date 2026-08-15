using Axlon.Services.Contracts.Base.CommonEnum;
using Axlon.Services.Contracts.Promotion.RootTkey;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using SqlSugar;

namespace Axlon.Services.Contracts.Promotion
{
    /// <summary>
    /// 分佣比例规则
    /// </summary>
    [Tenant("Main")]
    [SugarTable("promotion_commission_rules", "分佣比例规则")]
    public class PromotionCommissionRules : PromotionCommissionRulesRoot<long>
    {

        /// <summary>
        /// 规则类型：global/merchant/category
        /// </summary>
        [SugarColumn(ColumnName = "rule_type", ColumnDescription = "规则类型：global/merchant/category", Length = 20, IsNullable = false)]
        public string RuleType { get; set; }

        /// <summary>
        /// 一级分佣比例（如0.04表示4%）
        /// </summary>
        [SugarColumn(ColumnName = "level_1_rate", ColumnDescription = "一级比例（如0.04）", DecimalDigits = 4, IsNullable = false)]
        public decimal Level1Rate { get; set; }

        /// <summary>
        /// 二级分佣比例（如0.02表示2%）
        /// </summary>
        [SugarColumn(ColumnName = "level_2_rate", ColumnDescription = "二级比例", DecimalDigits = 4, IsNullable = false)]
        public decimal Level2Rate { get; set; }

        /// <summary>
        /// 三级分佣比例（如0.01表示1%）
        /// </summary>
        [SugarColumn(ColumnName = "level_3_rate", ColumnDescription = "三级比例", DecimalDigits = 4, IsNullable = false)]
        public decimal Level3Rate { get; set; }

        /// <summary>
        /// 四级分佣比例
        /// </summary>
        [SugarColumn(ColumnName = "level_4_rate", ColumnDescription = "四级比例", DecimalDigits = 4, IsNullable = false)]
        public decimal Level4Rate { get; set; }

        /// <summary>
        /// 五级分佣比例
        /// </summary>
        [SugarColumn(ColumnName = "level_5_rate", ColumnDescription = "五级比例", DecimalDigits = 4, IsNullable = false)]
        public decimal Level5Rate { get; set; }

        /// <summary>
        /// 总佣金池比例（如0.07表示7%）
        /// </summary>
        [SugarColumn(ColumnName = "total_commission_rate", ColumnDescription = "总佣金池比例（如0.07=7%）", DecimalDigits = 4, IsNullable = false)]
        public decimal TotalCommissionRate { get; set; }

        /// <summary>
        /// 状态：1启用/0停用
        /// </summary>
        [SugarColumn(ColumnName = "status", ColumnDataType = "tinyint", ColumnDescription = "1启用/0停用", IsNullable = false)]
        public DisableEnable Status { get; set; }
    }
}
