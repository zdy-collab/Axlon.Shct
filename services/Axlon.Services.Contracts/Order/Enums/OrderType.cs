using System.ComponentModel;

namespace Axlon.Services.Contracts.Order.Enums
{
    /// <summary>
    /// 订单类型
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// 堂食
        /// </summary>
        [Description("堂食")]
        dine_in,

        /// <summary>
        /// 自提
        /// </summary>
        [Description("自提")]
        takeout,

        /// <summary>
        /// 配送
        /// </summary>
        [Description("配送")]
        delivery,

        /// <summary>
        /// 团购
        /// </summary>
        [Description("团购")]
        group_buy
    }
}
