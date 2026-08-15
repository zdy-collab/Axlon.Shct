using Axlon.Framework.Data.IRepository.Base;
using Axlon.Framework.Data.IServices.Base;
using Axlon.Framework.Data.Services.Base;
using Axlon.Services.Contracts.Order;
using Axlon.Services.Order.Services.Interfaces;

namespace Axlon.Services.Order.Services
{
    public class WalletTransactionServices:BaseServices<WalletTransactions>,IWalletTransactionServices
    {
        public WalletTransactionServices(IBaseRepository<WalletTransactions> baseRepository) : base(baseRepository) 
        {

        }
    }
}
