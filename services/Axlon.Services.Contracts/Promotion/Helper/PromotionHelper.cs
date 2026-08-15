using Axlon.Services.Contracts.Promotion.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axlon.Services.Contracts.Promotion.Helper
{
    public class PromotionHelper
    {
        public static decimal GetCommissionRate(
        PromotionRelationsBasicDto relation,
        PromotionCommissionRulesBasicDto rule)
            {
                return relation.Level switch
                {
                    1 => rule.Level1Rate,
                    2 => rule.Level2Rate,
                    3 => rule.Level3Rate,
                    4 => rule.Level4Rate,
                    5 => rule.Level5Rate,
                    _ => 0m
                };
            }
    }
}
