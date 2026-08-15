using Axlon.Framework.Abstractions;
using Axlon.Framework.Core.DependencyInjection;

namespace Axlon.Services.Order.ServiceInvocation.Basic
{
    public interface IUserClient:IScopedDependency
    {
        Task<long?> GetPromotionIdAsync();
    }
}
