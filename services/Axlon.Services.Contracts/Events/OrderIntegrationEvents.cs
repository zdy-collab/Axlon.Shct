using Axlon.Framework.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axlon.Services.Contracts.Events
{
    public static class OrderTopics
    {
        public const string CompletedV1 = "axlon.order.completed.v1";
    }

    /// <summary>
    /// 订单完成
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="orderId"></param>
    public sealed record OrderCompletedIntegrationEvent(long userId, long orderId): IntegrationEvent;
}
