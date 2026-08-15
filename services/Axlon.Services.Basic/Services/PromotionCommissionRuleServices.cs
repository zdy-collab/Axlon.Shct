using Axlon.Framework.Data.IRepository.Base;
using Axlon.Framework.Data.IServices.Base;
using Axlon.Framework.Data.Services.Base;
using Axlon.Services.Basic.Services.Interfaces;
using Axlon.Services.Contracts.Promotion;
using Axlon.Services.Contracts.Promotion.Dto;
using Axlon.Services.Contracts.Promotion.Enums;
using Mapster;

namespace Axlon.Services.Basic.Services
{
    public class PromotionCommissionRuleServices: BaseServices<PromotionCommissionRules>,IPromotionCommissionRuleServices
    {
        public PromotionCommissionRuleServices(IBaseRepository<PromotionCommissionRules> pcrRepository):base(pcrRepository)
        {

        }

        public async Task<PromotionCommissionRulesBasicDto> ByMerchantIdGetPCRuleAsync(long merchantId)
        {
            // 先只查商家和全局
            var data = (await base.Query(whereExpression:
                x => (x.RuleType == PromotionCommissionRuleType.merchant.ToString() && x.RuleTargetId == merchantId)
                || x.RuleType == PromotionCommissionRuleType.global.ToString()));

            PromotionCommissionRulesBasicDto? res = null;

            if (data.Any(x => x.RuleType == PromotionCommissionRuleType.merchant.ToString()))
            {
                res = data.FirstOrDefault(x => x.RuleType == PromotionCommissionRuleType.merchant.ToString())
                    .Adapt<PromotionCommissionRulesBasicDto>();
            }
            else 
            {
                res = data.FirstOrDefault(x => x.RuleType == PromotionCommissionRuleType.global.ToString())
                    .Adapt<PromotionCommissionRulesBasicDto>();
            }
            if(res == null) throw new Exception("未配置佣金比例！");

            return res;
        }
    }
}
