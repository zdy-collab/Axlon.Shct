using Axlon.Framework.Data.IRepository.Base;
using Axlon.Framework.Data.Services.Base;
using Axlon.Services.Auth.IServices;
using Axlon.Services.Contracts.Order;

namespace Axlon.Services.Auth.Services
{
    public class UserWalletServices:BaseServices<UserWallets>,IUserWalletsServices
    {
        public UserWalletServices(IBaseRepository<UserWallets> baseRepository) : base(baseRepository) 
        {

        }
    }
}
