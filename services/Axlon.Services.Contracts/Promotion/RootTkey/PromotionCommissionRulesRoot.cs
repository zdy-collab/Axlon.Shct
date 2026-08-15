using Axlon.Services.Contracts.Base.BaseRoot;
using SqlSugar;

namespace Axlon.Services.Contracts.Promotion.RootTkey
{
    public class PromotionCommissionRulesRoot<Tkey> : BaseCreatedRoot<Tkey> where Tkey : IEquatable<Tkey>
    {
        /// <summary>
        /// 目标ID（merchant_id或category_id），全局规则时为0或null
        /// </summary>
        [SugarColumn(ColumnName = "rule_target_id", ColumnDescription = "目标ID（merchant_id或category_id）", IsNullable = true)]
        public Tkey? RuleTargetId { get; set; }
    }
}
