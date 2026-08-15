using Axlon.Framework.Abstractions;
using Axlon.Framework.Core.DependencyInjection;
using Axlon.Services.Contracts.Merchant.Dto;
using Axlon.Services.Contracts.Merchant.Dto.Inner;

namespace Axlon.Services.Order.ServiceInvocation.Merchant
{
    /// <summary>
    /// 商家服务
    /// </summary>
    public interface IMerchantClient: IScopedDependency
    {
        /// <summary>
        /// 获取商家基础信息
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="tableIds"></param>
        /// <returns></returns>
        Task<MerchantBasic_TableDto> GetMerchantBasicAsync(long merchantId, List<long>? tableIds = null);

    }
}
