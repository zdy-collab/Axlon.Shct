using Axlon.Framework.Data.IRepository.Base;
using Axlon.Framework.Data.Repository.Base;
using Axlon.Services.Contracts.Promotion;

namespace Axlon.Services.Order.Repository
{
    public interface IPromotionEarningRepository:IBaseRepository<PromotionEarnings>
    {
        /// <summary>
        /// 根据订单Id获取未处理的佣金记录
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<List<PromotionEarnings>> ByOrderIdGetPendingInfoAsync(long orderId);
    }
}
