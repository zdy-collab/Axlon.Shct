using Axlon.Services.Contracts.Base.CommonEnum;
using Axlon.Services.Contracts.Promotion.JsonObj;
using Axlon.Services.Contracts.Promotion.RootTkey;
using SqlSugar;

namespace Axlon.Services.Contracts.Promotion
{
    /// <summary>
    /// 推广等级规则
    /// </summary>
    [Tenant("Main")]
    [SugarTable("promotion_level_rules", "推广等级规则")]
    public class PromotionLevelRules : PromotionLevelRulesRoot<long>
    {

        /// <summary>
        /// 等级名称（普通/银牌/金牌/城市合伙人）
        /// </summary>
        [SugarColumn(ColumnName = "level_name", ColumnDescription = "等级名称", ColumnDataType = "varchar(50)", IsNullable = false)]
        public string LevelName { get; set; }

        /// <summary>
        /// 等级编码
        /// </summary>
        [SugarColumn(ColumnName = "level_code", ColumnDescription = "等级编码", ColumnDataType = "varchar(20)", IsNullable = false)]
        public PrompterLevel LevelCode { get; set; }

        /// <summary>
        /// 需要直推人数
        /// </summary>
        [SugarColumn(ColumnName = "require_direct_count", ColumnDescription = "需要直推人数", IsNullable = false)]
        public int RequireDirectCount { get; set; }

        /// <summary>
        /// 需要团队总人数
        /// </summary>
        [SugarColumn(ColumnName = "require_team_count", ColumnDescription = "需要团队总人数", IsNullable = false)]
        public int RequireTeamCount { get; set; }

        /// <summary>
        /// 需要团队总业绩
        /// </summary>
        [SugarColumn(ColumnName = "require_team_revenue", ColumnDescription = "需要团队总业绩", DecimalDigits = 2, IsNullable = false)]
        public decimal RequireTeamRevenue { get; set; }

        /// <summary>
        /// 权益说明（JSON格式存储）
        /// </summary>
        [SugarColumn(ColumnName = "benefits", ColumnDescription = "权益说明", IsNullable = true, IsJson = true)]
        public BenefitItem Benefits { get; set; }

        /// <summary>
        /// 排序（数值越小越靠前）
        /// </summary>
        [SugarColumn(ColumnName = "sort", ColumnDescription = "排序", IsNullable = false)]
        public int Sort { get; set; }
    }
}
