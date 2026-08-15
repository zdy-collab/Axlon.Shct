using System.ComponentModel;

namespace Axlon.Services.Contracts.Order.Enums
{
    public enum OrderRefundStatus
    {
        /// <summary>
        /// 待审批
        /// </summary>
        [Description("待审批")]
        pending,

        /// <summary>
        /// 已通过
        /// </summary>
        [Description("已通过")]
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
