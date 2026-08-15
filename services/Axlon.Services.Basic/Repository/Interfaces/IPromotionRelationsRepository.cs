using Axlon.Framework.Data.IRepository.Base;
using Axlon.Services.Contracts.Models;
using Axlon.Services.Contracts.Promotion;

namespace Axlon.Services.Basic.Repository.Interfaces
{
    public interface IPromotionRelationsRepository:IBaseRepository<PromotionRelations>
    {
        /// <summary>
        /// 根据parentId 获取所有上级
        /// </summary>
        /// <returns></returns>
        Task<List<PromotionRelations>> ByParentIdGetParent(long parentId);
    }
}
