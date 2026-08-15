using Axlon.Framework.Data.IRepository.Base;
using Axlon.Framework.Data.Services.Base;
using Axlon.Services.Contracts.Promotion;
using Axlon.Services.Contracts.Promotion.Dto;
using Axlon.Services.Order.ServiceInvocation.Basic;
using Axlon.Services.Order.Services.Interfaces;

namespace Axlon.Services.Order.Services
{
    public class PromotionCommissionRulesServices : BaseServices<PromotionCommissionRules>, IPromotionCommissionRulesServices
    {
        private readonly IBaseRepository<PromotionEarnings> promotionEarningsRepository;
        private readonly IOrderServices orderServices;
        private readonly IPromotionRelationClient promotionRelationClient;

        public PromotionCommissionRulesServices(IBaseRepository<PromotionCommissionRules> baseRepository,
            IBaseRepository<PromotionEarnings> promotionEarningsRepository,
            IOrderServices orderServices,
            IPromotionRelationClient promotionRelationClient) : base(baseRepository)
        {
            this.promotionEarningsRepository = promotionEarningsRepository;
            this.orderServices = orderServices;
            this.promotionRelationClient = promotionRelationClient;
        }
        public Task<bool> ReceiveCommissionAsync(long orderId)
        {
            var order = orderServices.QueryById(orderId);
            //var promotionRelations = promotionRelationClient.
            throw new NotImplementedException();
        }
    }
}
