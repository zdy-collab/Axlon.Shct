using Axlon.Framework.Abstractions;
using Axlon.Framework.Core.HttpContextUser;
using Axlon.Framework.Data.IRepository.Base;
using Axlon.Framework.Data.Services.Base;
using Axlon.Framework.Data.UnitOfWorks;
using Axlon.Framework.EventBus;
using Axlon.Services.Contracts.Base;
using Axlon.Services.Contracts.Events;
using Axlon.Services.Contracts.Extensions;
using Axlon.Services.Contracts.Models;
using Axlon.Services.Contracts.Order;
using Axlon.Services.Contracts.Order.Dto.WalletDto;
using Axlon.Services.Contracts.Order.Enums;
using Axlon.Services.Order.Repository;
using Axlon.Services.Order.Services.Interfaces;
using Serilog.Core;
using SqlSugar;
using StackExchange.Redis;

namespace Axlon.Services.Order.Services
{
    public class WalletServices : BaseServices<UserWallets>, IWalletServices
    {
        private readonly IUser loginUser;
        private readonly IWalletTransactionServices walletTransactionServices;
        private readonly IAxlonEventPublisher eventPublisher;
        private readonly ILogger<WalletServices> logger;
        private readonly IUnitOfWorkManage unitOfWorkManage;

        public WalletServices(IBaseRepository<UserWallets> repository, IUser loginUser, IAxlonEventPublisher eventPublisher,
            ILogger<WalletServices> logger, IWalletTransactionServices walletTransactionServices, IUnitOfWorkManage unitOfWorkManage) : base(repository)
        {
            this.loginUser = loginUser;
            this.eventPublisher = eventPublisher;
            this.logger = logger;
            this.walletTransactionServices = walletTransactionServices;
            this.unitOfWorkManage = unitOfWorkManage;
        }

        private List<string> earningsTypes = new()
        {
            WalletTransactionsType.commission.ToString(),
            WalletTransactionsType.first_order_cashback.ToString(),
            WalletTransactionsType.refund.ToString()
        };
        //private Expression<Func<WalletTransactions, bool>> earningsRevenueException =
        //x => x.Type.ToString() == WalletTransactionsType.commission.ToString()
        //|| x.Type.ToString() == WalletTransactionsType.first_order_cashback.ToString()
        //|| x.Type.ToString() == WalletTransactionsType.refund.ToString();

        public async Task<RevenueDto> GetMyRevenueDetailsAsync(QueryPage queryPage)
        {
            RefAsync<int> total = 0;
            var revenueDetails = await base.Db.Queryable<WalletTransactions>()
                .LeftJoin<Orders>((x, y) => x.OrderId == y.Id)
                .LeftJoin<SysUserInfo>((x, y, z) => y.UserId == z.Id)
                .LeftJoin<SysUserInfo>((x, y, z, h) => x.UserId == h.Id)
                .Where(x => x.UserId == loginUser.ID && earningsTypes.Contains(x.Type.ToString()))
                .OrderByDescending(x => x.CreatedAt)
                .Select((x, y, z, h) => new RevenueDetailsDto
                {
                    Name = SqlFunc.IsNull(z.Nickname, h.Nickname),
                    Type = x.Type,
                    CreatedAt = x.CreatedAt,
                    Amount = x.Amount
                })
                .ToPageListAsync(queryPage.page, queryPage.pageSize, total);


            //throw new Exception();
            var res = new RevenueDto(queryPage.page, total, queryPage.pageSize, revenueDetails);

            res.MyDivideInto = await base.Db.Queryable<WalletTransactions>()
                .Where(x => x.Type.ToString() == WalletTransactionsType.commission.ToString())
                .SumAsync(x => x.Amount);

            return res;
        }

        public async Task<GetMyWalletInfoRes> GetMyWalletInfoAsync()
        {
            if (loginUser.GetIsVisitor())
            {
                return new GetMyWalletInfoRes();
            }

            var userWallet = (await base.Query(whereExpression: x => x.UserId == loginUser.ID)).FirstOrDefault();
            var res = new GetMyWalletInfoRes();
            if (userWallet != null)
            {
                res.Balance = userWallet.Balance;
                res.FrozenBalance = userWallet.FrozenBalance;



                // 获取本月收益
                res.MonthEarnings = await base.Db.Queryable<WalletTransactions>()
                    .Where(x => x.UserId == loginUser.ID && x.CreatedAt.Month == DateTime.Now.Month && earningsTypes.Contains(x.Type.ToString()))
                    .SumAsync(x => x.Amount);

                // 获取今日收益钱包流水
                res.TodayEarnings = await base.Db.Queryable<WalletTransactions>()
                    .Where(x => x.UserId == loginUser.ID && x.CreatedAt.Date == DateTime.Today && earningsTypes.Contains(x.Type.ToString()))
                    .SumAsync(x => x.Amount);

                // 获取总收益
                res.TotalEarnings = await base.Db.Queryable<WalletTransactions>()
                    .Where(x => x.UserId == loginUser.ID && earningsTypes.Contains(x.Type.ToString()))
                    .SumAsync(x => x.Amount);

                res.revenueBreakdown = await base.Db.Queryable<WalletTransactions>()
                    .LeftJoin<Orders>((x, y) => x.OrderId == y.Id)
                    .LeftJoin<SysUserInfo>((x, y, z) => y.UserId == z.Id)
                    .LeftJoin<SysUserInfo>((x, y, z, h) => x.UserId == h.Id)
                    .Where(x => x.UserId == loginUser.ID && earningsTypes.Contains(x.Type.ToString()))
                    .OrderByDescending(x => x.CreatedAt)
                    .Take(3)
                    .Select((x, y, z, h) => new RevenueDetailsDto
                    {
                        Name = SqlFunc.IsNull(z.Nickname, h.Nickname),
                        Type = x.Type,
                        CreatedAt = x.CreatedAt,
                        Amount = x.Amount
                    })
                    .ToListAsync();

                //res.PrompterLevel = (await base.Db.Queryable<SysUserInfo>().FirstAsync(x => x.Id == loginUser.ID)).PrompterLevel.ToString();
            }
            return res;
        }

        public async Task<bool> ByPromotionEarningAddIncomeAsync(PromotionEarningCalculatedIntegrationEvent @event, CancellationToken cancellationToken = default)
        {
            try
            {
                unitOfWorkManage.BeginTran();

                var walletTransactions = new List<WalletTransactions>();

                // 获取该订单未处理的分佣记录
                var earnings = @event.earnings;

                // 获取该订单得到佣金的用户
                var userIds = earnings.Select(x => x.userId).ToList();

                // 获取该订单得到佣金的用户钱包
                var wallets = await base.Query(x => userIds.Contains(x.UserId));

                //var myWallet = wallets.First(x => x.UserId == order.UserId);

                foreach (var item in earnings)
                {
                    // 获取推广人钱包信息
                    var itemWallet = wallets.First(x => x.UserId == item.userId);

                    walletTransactions.Add(WalletTransactions.Create(item.userId, item.orderId, WalletTransactionsType.commission, item.commissionAmount, itemWallet.Balance));

                    // 更新钱包余额
                    itemWallet.Balance += item.commissionAmount;

                    // 结算

                    //item.SettleAccounts();
                }

                //await promotionEarningServices.Update(orderPromotionEarnings);

                await base.Update(wallets);

                await walletTransactionServices.BatchAddReturnRowsAsync(walletTransactions);

                unitOfWorkManage.CommitTran();

                // 发布分佣结算
                await TryPublishPromotionEarningSettledAsync(earnings.Select(x => x.promotionEarningId).ToList());

                return true;
            }
            catch (Exception ex) 
            {
                unitOfWorkManage.RollbackTran();
                return false;
            }
        }

        /// <summary>
        /// 发布佣金结算事件
        /// </summary>
        /// <param name="userId">创建订单用户Id</param>
        /// <param name="orderId">订单Id</param>
        /// <returns></returns>
        private async Task TryPublishPromotionEarningSettledAsync(List<long> promotionEarningIds)
        {

            try
            {
                await eventPublisher.PublishAsync(
                    PromotionEarningTopics.SettledV1,
                    new PromotionEarningSettledIntegrationEvent(promotionEarningIds));
            }
            catch (Exception exception)
            {
                logger.LogWarning(exception,
                    "发布佣金结算失败，{promotionEarningIds}",string.Join("/",promotionEarningIds));
            }
        }
    }
}
