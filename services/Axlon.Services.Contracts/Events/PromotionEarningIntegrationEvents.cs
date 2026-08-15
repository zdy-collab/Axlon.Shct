using Axlon.Framework.Abstractions.Messaging;
using Axlon.Services.Contracts.Promotion;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Axlon.Services.Contracts.Events
{
    public static class PromotionEarningTopics
    {
        /// <summary>
        /// 佣金计算完成
        /// </summary>
        public const string CalculatedV1 = "axlon.promotion-earning.calculated.v1";

        /// <summary>
        /// 申请佣金计算
        /// </summary>
        public const string CalculateV1 = "axlon.promotion-earning.calculate.v1";

        /// <summary>
        /// 佣金结算完成
        /// </summary>
        public const string SettledV1 = "axlon.promotion-earning.settled.v1";
    }

    /// <summary>
    /// 佣金计算完成
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="orderId"></param>
    public sealed record PromotionEarningCalculatedIntegrationEvent(IReadOnlyCollection<PromotionEarningCalculatedItem> earnings) : IntegrationEvent;

    public sealed record PromotionEarningCalculatedItem(long promotionEarningId, long userId, long orderId, long fromUserId, byte Level, decimal commissionAmount);


    /// <summary>
    /// 请求计算佣金
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="orderId"></param>
    public sealed record PromotionEarningCalculateIntegrationEvent(long userId, long orderId) : IntegrationEvent;

    /// <summary>
    /// 佣金结算
    /// </summary>
    /// <param name="promotionEarningIds"></param>
    public sealed record PromotionEarningSettledIntegrationEvent(List<long> promotionEarningIds) : IntegrationEvent;
}
