using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axlon.Services.Contracts.Promotion.Dto
{
    public class PromotionCommissionRulesBasicDto
    {
        public long Id { get; set; }

        /// <summary>
        /// 目标ID（merchant_id或category_id），全局规则时为0或null
        /// </summary>
        public long? RuleTargetId { get; set; }

        /// <summary>
        /// 规则类型：global/merchant/category
        /// </summary>
        public string RuleType { get; set; }

        /// <summary>
        /// 一级分佣比例（如0.04表示4%）
        /// </summary>
        public decimal Level1Rate { get; set; }

        /// <summary>
        /// 二级分佣比例（如0.02表示2%）
        /// </summary>
        public decimal Level2Rate { get; set; }

        /// <summary>
        /// 三级分佣比例（如0.01表示1%）
        /// </summary>
        public decimal Level3Rate { get; set; }

        /// <summary>
        /// 四级分佣比例
        /// </summary>
        public decimal Level4Rate { get; set; }

        /// <summary>
        /// 五级分佣比例
        /// </summary>
        public decimal Level5Rate { get; set; }

        /// <summary>
        /// 总佣金池比例（如0.07表示7%）
        /// </summary>
        public decimal TotalCommissionRate { get; set; }
    }
}
