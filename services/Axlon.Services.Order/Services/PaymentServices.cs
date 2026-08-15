using Axlon.Framework.Core.HttpContextUser;
using Axlon.Framework.Data.IRepository.Base;
using Axlon.Framework.Data.UnitOfWorks;
using Axlon.Services.Contracts.Base.CommonEnum;
using Axlon.Services.Contracts.Order;
using Axlon.Services.Contracts.Order.Dto.PaymentDto;
using Axlon.Services.Contracts.Order.Enums;
using Axlon.Services.Contracts.Promotion;
using Axlon.Services.Contracts.Promotion.Dto;
using Axlon.Services.Contracts.Promotion.Helper;
using Axlon.Services.Contracts.Promotion.RootTkey;
using Axlon.Services.Order.Repository;
using Axlon.Services.Order.ServiceInvocation.Basic;
using Axlon.Services.Order.Services.Interfaces;
using Dm.util;

namespace Axlon.Services.Order.Services
{
    public class PaymentServices : IPaymentServices
    {
        private readonly IOrderServices orderServices;
        private readonly ISysUserInfoServices sysUserInfoServices;
        private readonly IPromotionRelationClient promotionRelationClient;
        private readonly IPromotionCommissionRuleClient pcrClient;
        private readonly IUser loginUser;
        private readonly IPromotionEarningServices promotionEarningServices;
        private readonly IPromotionEarningRepository promotionEarningRepository;
        private readonly IWalletServices walletServices;
        private readonly ISysUserInfoServices userInfoServices;
        private readonly IWalletTransactionServices walletTransactionServices;
        private readonly IUnitOfWorkManage unitOfWorkManage;

        public PaymentServices(IOrderServices orderServices, ISysUserInfoServices sysUserInfoServices,
            IPromotionRelationClient promotionRelationClient, IPromotionCommissionRuleClient pcrClient, IUser loginUser,
            IPromotionEarningServices promotionEarningServices, IPromotionEarningRepository promotionEarningRepository,
            IWalletServices walletServices, ISysUserInfoServices userInfoServices, IWalletTransactionServices walletTransactionServices, IUnitOfWorkManage unitOfWorkManage)
        {
            this.orderServices = orderServices;
            this.sysUserInfoServices = sysUserInfoServices;
            this.promotionRelationClient = promotionRelationClient;
            this.pcrClient = pcrClient;
            this.loginUser = loginUser;
            this.promotionEarningServices = promotionEarningServices;
            this.promotionEarningRepository = promotionEarningRepository;
            this.walletServices = walletServices;
            this.userInfoServices = userInfoServices;
            this.walletTransactionServices = walletTransactionServices;
            this.unitOfWorkManage = unitOfWorkManage;
        }

        public async Task<bool> PayOrderAsync(PayOrderReq req)
        {
            try
            {
                unitOfWorkManage.BeginTran();
                var order = await orderServices.QueryById(req.orderId);

                // 后期返回Message
                if (order.Status != OrderStatus.PendingPayment.ToString()) throw new Exception("该订单已支付");
                #region 假如支付成功

                order.PaidSuccess();

                await orderServices.Update(order);

                #endregion

                var user = await sysUserInfoServices.QueryById(order.UserId);

                // 消费者钱包
                var myWallet = (await walletServices.Query(x => x.UserId == order.UserId)).First();

                // 如果具有推广关系，目前不会刷新推广信息
                if (user.RegisterFromUserId != null && user.RegisterFromUserId > 0) 
                {

                    #region 推广关系绑定

                    // 只有首次消费，才会绑定推广关系，先临时这么写
                    if (user.FirstOrderCompleted == YesNo.否)
                    {
                        var messageModel = await promotionRelationClient.BindPromotionRelationAsync(new Contracts.Promotion.Dto.BindPromotionRelationReq
                        {
                            UserId = order.UserId,
                            ParentId = user.RegisterFromUserId,
                            BindOrderId = req.orderId
                        });
                        if (messageModel.success)
                        {
                            user.FirstOrderCompleted = YesNo.是;
                            await sysUserInfoServices.Update(user);
                        }
                        //绑定推广关系失败
                        else return false;
                    }
                    #endregion

                    #region 分佣计算

                    // 获取推广关系
                    var prcMessage = await promotionRelationClient.ByUserIdGetBasicInfoAsync(loginUser.ID);

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

                    #endregion

                    #region 钱包收益流水

                    var walletTransactions = new List<WalletTransactions>();

                    // 获取该订单未处理的分佣记录
                    var orderPromotionEarnings = await promotionEarningRepository.ByOrderIdGetPendingInfoAsync(order.Id);

                    // 获取该订单得到佣金的用户
                    var userIds = orderPromotionEarnings.Select(x => x.UserId).ToList();

                    // 获取该订单得到佣金的用户钱包
                    var wallets = await walletServices.Query(x => userIds.Contains(x.UserId));

                    //var myWallet = wallets.First(x => x.UserId == order.UserId);

                    walletTransactions.Add(WalletTransactions.Create(order.UserId, order.Id, WalletTransactionsType.consumption, order.PaidAmount, myWallet.Balance));
                    foreach (var item in orderPromotionEarnings)
                    {
                        // 获取推广人钱包信息
                        var itemWallet = wallets.First(x => x.UserId == item.UserId);

                        walletTransactions.Add(WalletTransactions.Create(item.UserId, order.Id, WalletTransactionsType.commission, item.CommissionAmount, itemWallet.Balance));

                        // 更新钱包余额
                        itemWallet.Balance += item.CommissionAmount;

                        // 结算
                        item.SettleAccounts();
                    }

                    await promotionEarningServices.Update(orderPromotionEarnings);

                    await walletServices.Update(wallets);

                    await walletTransactionServices.BatchAddReturnRowsAsync(walletTransactions);

                }

                // 判断消费者钱包余额是否大于消费金额，如果大于则扣除，如果小于，那么用户就是银联支付
                if (myWallet.Balance >= order.PaidAmount) myWallet.Balance = myWallet.Balance - order.PaidAmount;

                await walletServices.Update(myWallet);

                // 订单完成
                order.Completed();
                await orderServices.Update(order);
                unitOfWorkManage.CommitTran();
                return true;
            }
            catch (Exception ex) 
            {
                unitOfWorkManage.RollbackTran();
                return false;
                
            }

            #endregion
            throw new NotImplementedException();
        }
    }
    //public class Transaction 
    //{

    //}
}
