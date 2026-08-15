using System.ComponentModel;

namespace Axlon.Services.Contracts.Promotion.Enums
{
    public enum PromotionEarningsStatus
    {
        /// <summary>
        /// 待处理
        /// </summary>
        [Description("待处理")]
        pending,
        /// <summary>
        /// 已结算
        /// </summary>
        [Description("已结算")]
        settled,
        /// <summary>
        /// 已取消
        /// </summary>
        [Description("cancelled")]
        cancelled
    }
}
