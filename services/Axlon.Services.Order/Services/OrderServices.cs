using Axlon.Framework.Abstractions;
using Axlon.Framework.Core.Extensions;
using Axlon.Framework.Core.HttpContextUser;
using Axlon.Framework.Data.IRepository.Base;
using Axlon.Framework.Data.IRepository.Base;
using Axlon.Framework.Data.Services.Base;
using Axlon.Framework.Data.Services.Base;
using Axlon.Framework.Data.UnitOfWorks;
using Axlon.Framework.EventBus;
using Axlon.Services.Contracts.Events;
using Axlon.Services.Contracts.Extensions;
using Axlon.Services.Contracts.Merchant.Dto;
using Axlon.Services.Contracts.Order;
using Axlon.Services.Contracts.Order.Dto.OrderDto;
using Axlon.Services.Contracts.Order.Enums;
using Axlon.Services.Contracts.Product;
using Axlon.Services.Order.Repository;
using Axlon.Services.Order.ServiceInvocation.Basic;
using Axlon.Services.Order.ServiceInvocation.Merchant;
using Axlon.Services.Order.Services.Interfaces;
using Mapster;
using Serilog.Core;
using System.Linq.Expressions;

namespace Axlon.Services.Order.Services
{
    public class OrderServices : BaseServices<Orders>, IOrderServices
    {
        private readonly IMerchantClient merchantClient;
        private readonly IProductClient productClient;
        private readonly IUnitOfWorkManage _unitOfWorkManage;
        private readonly IUserClient userClient;
        private readonly IUser loginUser;
        private readonly ILogger<OrderServices> logger;
        private readonly IAxlonEventPublisher eventPublisher;

        /// <summary>
        /// 具有菜品的订单类型
        /// </summary>
        private readonly List<OrderType> productType = new(){ OrderType.dine_in, OrderType.takeout, OrderType.delivery };
        //private readonly ISysUserInfoServices sysUserInfoServices;

        public OrderServices(IBaseRepository<Orders> repository, IUser loginUser, ISysUserInfoServices sysUserInfoServices,
            IMerchantClient merchantClient, IProductClient productClient, IUnitOfWorkManage unitOfWorkManage
            , IUserClient userClient, ILogger<OrderServices> logger, IAxlonEventPublisher eventPublisher) : base(repository)
        {
            this.loginUser = loginUser;
            this.merchantClient = merchantClient;
            this.productClient = productClient;
            _unitOfWorkManage = unitOfWorkManage;
            this.userClient = userClient;
            this.logger = logger;
            this.eventPublisher = eventPublisher;
        }

        private static string GenerateOrderNo(OrderType type)
        {
            var orderNo = "";
            switch (type)
            {
                case OrderType.dine_in:
                    orderNo = "TS";
                    break;
                case OrderType.takeout:
                    orderNo = "ZT";
                    break;
                case OrderType.delivery:
                    orderNo = "PS";
                    break;
                case OrderType.group_buy:
                    orderNo = "TG";
                    break;
            }
            // 格式：取GUID前8位 + 日期时间
            var guid = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            return $"{timestamp}{guid}";
        }

        public async Task<CreateOrderRes> CreateOrderAsync(CreateOrderReq req)
        {
            //var msg = new MessageModel<CreateOrderRes>();

            #region 数据校验
            //var msg = string.Empty;

            var productIds = req.orderItems.Select(x => x.ProductId).ToList();
            var products = await productClient.ByIdsGetProductsAsync(productIds);



            #region 菜品归属校验

            if (productType.Any(x => x == req.Type))
            {
                var disMerchantProducts = products.Where(x => x.MerchantId != req.MerchantId);
                if (disMerchantProducts.Count() > 0)
                {
                    throw new Exception("以下菜品不属于本商家：" + string.Join(",", disMerchantProducts.Select(x => x.Name).ToList()));
                }
            }

            #endregion

            #region 商家营业时间校验
            //var merchantInfo = await merchantClient.GetMerchantBasicAsync(req.MerchantId);

            //DateTime now = DateTime.Now;
            //int hour = now.Hour;
            //int minute = now.Minute;
            //int second = now.Second;

            //DayOfWeek dayOfWeek = now.DayOfWeek;
            ////merchantInfo.BusinessHours.Friday
            #endregion

            #endregion
            var promotionUserId = await userClient.GetPromotionIdAsync();   // 获取推广人Id
            var source = req.TableId != null ? OrderSource.桌码 : promotionUserId != null ? OrderSource.推广码 : OrderSource.小程序;
            //var 
            var orderCommand = new CreateOrderCommand()
            {
                UserId = loginUser.ID,
                MerchantId = req.MerchantId,
                TableId = req.TableId,
                PromoUserId = promotionUserId,
                Source = source,
                Type = req.Type,
                OrderNo = GenerateOrderNo(req.Type),
                TotalAmount = products.Sum(x => x.Price),
                orderItems = products.Select(x => new CreateOrderItemCommand
                {
                    ProductId = x.Id,
                    ProductImageFileId = x.ImageFileId,
                    ProductImageOss = x.ImageOss,
                    ProductName = x.Name,
                    ProductPrice = x.Price,
                    Quantity = req.orderItems.FirstOrDefault(y => y.ProductId == x.Id)?.Quantity ?? 1,
                    TotalPrice = x.Price * (req.orderItems.FirstOrDefault(y => y.ProductId == x.Id)?.Quantity ?? 1),
                    Remarks = req.orderItems.FirstOrDefault(y => y.ProductId == x.Id)?.Remarks
                }).ToList()
            };
            var order = Orders.Create(orderCommand);
            try
            {
                _unitOfWorkManage.BeginTran();

                long orderId = await base.Add(order);

                foreach (var item in order.orderItems)
                {
                    item.OrderId = orderId;
                }

                await base.Db.Insertable(order.orderItems).ExecuteCommandAsync();

                _unitOfWorkManage.CommitTran();

                return  new CreateOrderRes { OrderId = orderId };
            }
            catch (Exception ex)
            {
                _unitOfWorkManage.RollbackTran();

                throw new Exception("失败");
            }
        }

        public async Task<GetMyOderDetailRes> GetMyOderDetailsAsync(long orderId)
        {

            var order = await base.QueryById(orderId);
            var res = order.Adapt<GetMyOderDetailRes>();

            // 具有菜品的订单
            if (productType.Any(x => x.ToString() == order.Type))
            {
                var merchantInfo = await merchantClient.GetMerchantBasicAsync(order.MerchantId, new List<long> { order.TableId });

                res.products = (await base.Db.Queryable<OrderItems>()
                    .Where(x => x.OrderId == orderId)
                    .Select(x => new GMODD_ProductDto 
                    {
                        Id  = x.Id,
                        ImageFileId = x.ProductImageFileId,
                        ImageOss = x.ProductImageOss,
                        ProductName = x.ProductName,
                        ProductPrice = x.ProductPrice,
                        Quantity = x.Quantity,
                        TotalPrice = x.TotalPrice
                    })
                    .ToListAsync());

                res.Merchant = merchantInfo.Adapt<GMODD_MerchantDto>();
                res.discounts = new List<GMODD_Discount>()
                {
                    new GMODD_Discount {Title = "平台补贴立减",Amount = -3 },
                    new GMODD_Discount {Title = "邻里专属折扣",Amount = -1 }

                };

                if(merchantInfo.merchantTables != null && merchantInfo.merchantTables.Count > 0) res.MerchantTable = merchantInfo.merchantTables[0].Adapt<GMODD_MerchantTable>();
                return res;
            }
            else if(order.Type == OrderType.group_buy.ToString())// 团购
            {
                return res;
            }

            throw new Exception("该订单类型暂未实现");
        }

        public async Task<PageResponseModel<GetMyOrdersRes>> GetMyOrdersAsync(GetMyOrdersReq req)
        {
            var isVisitor = loginUser.GetIsVisitor();
            if (isVisitor) return new PageResponseModel<GetMyOrdersRes>();

            List<string> orderStatus = new();
            List<string> orderPayStatus = new();
            if (req.status != null)
            {
                if (req.status == QueryOrderStatus.待付款)
                {
                    orderStatus.Add(OrderStatus.PendingPayment.ToString()); // 待支付
                }
                else if (req.status == QueryOrderStatus.待使用)
                {
                    orderStatus.Add(OrderStatus.PendingUsage.ToString());   // 待使用

                }
                else if (req.status == QueryOrderStatus.进行中)
                {
                    orderStatus.AddRange(
                        OrderStatus.Paid.ToString(),    // 已支付
                        OrderStatus.Preparing.ToString(),   // 备餐中
                        OrderStatus.Served.ToString(),  // 已出餐
                        OrderStatus.Dining.ToString(),  // 用餐中
                        OrderStatus.ReadyForPickup.ToString(),  // 待取餐
                        OrderStatus.PendingDelivery.ToString(),  // 待配送
                        OrderStatus.Delivering.ToString(),  // 配送中
                        OrderStatus.Delivered.ToString()    // 已送达
                    );
                }
                else if (req.status == QueryOrderStatus.已完成)
                {
                    orderStatus.AddRange(
                        OrderStatus.Completed.ToString(),   // 已完成
                        OrderStatus.PickedUp.ToString(),    // 已取餐
                        OrderStatus.Delivered.ToString(),   // 已送达
                        OrderStatus.Verified.ToString() // 已核销
                    );
                }
                else if (req.status == QueryOrderStatus.已取消)
                {
                    orderStatus.AddRange(
                        OrderStatus.Cancelled.ToString(),   // 已取消
                        OrderStatus.Refunded.ToString() // 已退款
                    );
                }

                if (req.status == QueryOrderStatus.退款中)
                {
                    orderPayStatus.Add(OrderPayStatus.refunding.ToString());
                }
            }
            Expression<Func<Orders, bool>> orderExpression = x => x.UserId == loginUser.ID;
            if (orderStatus.Count > 0) orderExpression = orderExpression.And(x => orderStatus.Contains(x.Status));
            if (orderPayStatus.Count > 0) orderExpression = orderExpression.And(x => orderPayStatus.Contains(x.PayStatus));
            var orders = await base.QueryPage(
                whereExpression: orderExpression,
                pageIndex: req.page,
                pageSize: req.pageSize,
                orderByFileds: "created_at");

            var orderIds = orders.data.Select(x => x.Id).ToList();

            var orderItems = await base.Db.Queryable<OrderItems>().Where(x => orderIds.Contains(x.OrderId)).ToListAsync();

            var orderItemsDict = orderItems
                .GroupBy(x => x.OrderId)
                .ToDictionary(x => x.Key, x => x.ToList());
            var a = orderItemsDict.GetValueOrDefault(1);
            orders.data.ForEach(x => x.orderItems = orderItemsDict.GetValueOrDefault(x.Id) ?? new List<OrderItems>());

            var res = orders.Adapt<PageResponseModel<GetMyOrdersRes>>();

            return res;
        }

        public async Task<bool> OrderCompletedAsync(long orderId)
        {
            var userId = (await base.QueryById(orderId)).UserId;

            // 发布订单完成事件
            await TryPublishOrderCompletedAsync(userId, orderId);

            return true;
        }

        /// <summary>
        /// 发布订单完成事件
        /// </summary>
        /// <param name="userId">创建订单用户Id</param>
        /// <param name="orderId">订单Id</param>
        /// <returns></returns>
        private async Task TryPublishOrderCompletedAsync(long userId,long orderId)
        {

            try
            {
                await eventPublisher.PublishAsync(
                    OrderTopics.CompletedV1,
                    new OrderCompletedIntegrationEvent(userId, orderId));
            }
            catch (Exception exception)
            {
                logger.LogWarning(exception,
                    "发布订单完成失败，UserId={UserId}, OrderId={MerchantId}", userId, orderId);
            }
        }

    }
}
