using System.ComponentModel;

namespace Axlon.Services.Contracts.Promotion.Enums
{
    public enum PromotionWithdrawalsStatus
    {
        /// <summary>
        /// 待处理
        /// </summary>
        [Description("待处理")]
        pending,

        /// <summary>
        /// 已批准
        /// </summary>
        [Description("已批准")]
        approved,

        /// <summary>
        /// 已拒绝
        /// </summary>
        [Description("已拒绝")]
        rejected,

        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        completed
    }
}
