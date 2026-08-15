using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Axlon.Services.Contracts.Promotion.Enums
{
    public enum PromotionCommissionRuleType
    {
        /// <summary>
        /// 全局
        /// </summary>
        [Description("全局")]
        global,

        /// <summary>
        /// 商家
        /// </summary>
        [Description("商家")]
        merchant,

        /// <summary>
        /// 品类
        /// </summary>
        [Description("品类")]
        category
    }
}
