using Axlon.Framework.Abstractions;
using Axlon.Framework.Data.IServices.Base;
using Axlon.Services.Basic.Input;
using Axlon.Services.Basic.Output;
using Axlon.Services.Contracts.User;

namespace Axlon.Services.Basic.Services.Interfaces
{
    public interface IUserAddressServices : IBaseServicesExtend<UserAddressAddInput, UserAddressEditInput, UserAddressOutput, UserAddress>
    {

        Task<bool> SetDefaultAsync(long id);

        Task<PageResponseModel<UserAddressOutput>> GetPageAsync(UserAddressPageInput pageRequest);
    }
}
