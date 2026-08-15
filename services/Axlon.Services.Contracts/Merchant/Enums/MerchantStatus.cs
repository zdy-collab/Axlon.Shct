using System.ComponentModel;

namespace Axlon.Services.Contracts.Merchant.Enums
{
    /// <summary>
    /// 门店状态枚举
    /// </summary>
    public enum MerchantStatus
    {
        [Description("待审核")]
        待审核 = 0,

        [Description("已通过")]
        已通过 = 1,

        [Description("已驳回")]
        已驳回 = 2,

        [Description("已冻结")]
        已冻结 = 3,
    }
}
