using System.ComponentModel;

namespace Axlon.Services.Contracts.Order.Enums
{
    /// <summary>
    /// 订单状态
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>待确认</summary>
        [Description("待确认")]
        WaitConfirmed = -1,

        /// <summary>待支付</summary>
        [Description("待支付")]
        PendingPayment = 0,

        /// <summary>已支付</summary>
        [Description("已支付")]
        Paid = 1,

        /// <summary>备餐中</summary>
        [Description("备餐中")]
        Preparing = 2,

        #region 堂食专属

        /// <summary>
        /// 已出餐
        /// </summary>
        [Description("已出餐")]
        Served = 10,

        /// <summary>
        /// 用餐中
        /// </summary>
        [Description("用餐中")]
        Dining = 11,

        #endregion

        #region 自提专属

        /// <summary>
        /// 待取餐
        /// </summary>
        [Description("待取餐")]
        ReadyForPickup = 20,

        /// <summary>
        /// 已取餐
        /// </summary>
        [Description("已取餐")]
        PickedUp = 21,

        #endregion

        #region 配送专属

        /// <summary>
        /// 待配送
        /// </summary>
        [Description("待配送")]
        PendingDelivery = 30,

        /// <summary>
        /// 配送中
        /// </summary>
        [Description("配送中")]
        Delivering = 31,

        /// <summary>
        /// 已送达
        /// </summary>
        [Description("已送达")]
        Delivered = 32,

        #endregion

        #region 团购专属

        /// <summary>
        /// 待使用
        /// </summary>
        [Description("待使用")]
        PendingUsage = 40,

        /// <summary>
        /// 已核销
        /// </summary>
        [Description("已核销")]
        Verified = 41,

        #endregion

        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        Completed = 100,

        /// <summary>
        /// 已取消
        /// </summary>
        [Description("已取消")]
        Cancelled = -1,

        /// <summary>
        /// 已退款
        /// </summary>
        [Description("已退款")]
        Refunded = -2
    }
}
