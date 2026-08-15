using Axlon.Framework.Data.Services.Base;
using Axlon.Framework.EventBus;
using Axlon.Services.Contracts.Events;
using Axlon.Services.Contracts.Promotion;
using Axlon.Services.Contracts.Promotion.Helper;
using Axlon.Services.Order.Repository;
using Axlon.Services.Order.ServiceInvocation.Basic;
using Axlon.Services.Order.Services.Interfaces;
using Serilog.Core;
using StackExchange.Redis;
using System.Net.Sockets;

namespace Axlon.Services.Order.Services
{
    public class PromotionEarningServices : BaseServices<PromotionEarnings>, IPromotionEarningServices
    {
        private readonly IPromotionRelationClient promotionRelationClient;
        private readonly IPromotionCommissionRuleClient pcrClient;
        private readonly IOrderServices orderServices;
        private readonly IPromotionEarningServices promotionEarningServices;
        private readonly IAxlonEventPublisher eventPublisher;
        private readonly ILogger<PromotionEarningServices> logger;
        public PromotionEarningServices(IPromotionEarningRepository repository, IPromotionRelationClient promotionRelationClient,
            IPromotionCommissionRuleClient pcrClient, IOrderServices orderServices, IPromotionEarningServices promotionEarningServices,
            IAxlonEventPublisher eventPublisher, ILogger<PromotionEarningServices> logger) : base(repository)
        {
            this.promotionRelationClient = promotionRelationClient;
            this.pcrClient = pcrClient;
            this.orderServices = orderServices;
            this.promotionEarningServices = promotionEarningServices;
            this.eventPublisher = eventPublisher;
            this.logger = logger;
        }

        public async Task<bool> PromotionEarningCreateAsync(PromotionEarningCalculateIntegrationEvent @event, CancellationToken cancellationToken = default)
        {
            // 获取订单信息
            var order = await orderServices.QueryById(@event.orderId);

            // 获取推广关系
            var prcMessage = await promotionRelationClient.ByUserIdGetBasicInfoAsync(order.UserId);

            // 获取分佣规则
            var pcrRuleMessage = await pcrClient.ByMerchantIdGetPCRuleAsync(order.MerchantId);
            if (!pcrRuleMessage.success) throw new Exception("获取佣金规则失败");
            var pcrRule = pcrRuleMessage.response;

            if (!pcrRuleMessage.success) throw new Exception("获取推广等级失败");

            var promotionEarnings = new List<PromotionEarnings>();

            foreach (var item in prcMessage.response)
            {
                // 获取分佣比例
                var commissionRate = PromotionHelper.GetCommissionRate(item, pcrRuleMessage.response);

                // 订单金额
                var orderAmount = order.PaidAmount;

                // 获取佣金金额
                var commissionAmount = order.PaidAmount * commissionRate;

                // 添加分佣记录
                promotionEarnings.Add(PromotionEarnings.Create(order.Id, item.ParentId, order.UserId, item.Level, commissionAmount, orderAmount, commissionRate));
            }

            await promotionEarningServices.Add(promotionEarnings);

            // 发布分佣记录已创建事件
            await TryPublishPromotionEarningCreatedAsync(promotionEarnings);

            return true;
        }

        /// <summary>
        /// 发布佣金记录计算完成事件
        /// </summary>
        /// <param name="userId">创建订单用户Id</param>
        /// <param name="orderId">订单Id</param>
        /// <returns></returns>
        private async Task TryPublishPromotionEarningCreatedAsync(List<PromotionEarnings> earnings)
        {

            try
            {
                var dto = earnings.Select(x => new PromotionEarningCalculatedItem(x.Id, x.UserId, x.OrderId, x.FromUserId, x.Level, x.CommissionAmount)).ToList();

                await eventPublisher.PublishAsync(
                    PromotionEarningTopics.CalculatedV1,
                    new PromotionEarningCalculatedIntegrationEvent(dto));
            }
            catch (Exception exception)
            {
                //logger.LogWarning(exception,
                //    "发布佣金记录计算完成失败，UserId={UserId}, OrderId={MerchantId}", userId, orderId);
            }
        }
    }
}
