using Axlon.Services.Basic.Services.Interfaces;
using Axlon.Services.Contracts.Events;
using DotNetCore.CAP;

namespace Axlon.Services.Basic.EventHandlers.user;

public sealed class UserFootprintViewedIntegrationEventHandler(IUserFootprintServices services) : ICapSubscribe
{
    [CapSubscribe(UserFootprintTopics.ViewedV1)]
    public async Task HandleAsync(
        UserFootprintViewedIntegrationEvent @event, CancellationToken cancellationToken = default) =>
        _ = await services.RecordViewAsync(@event, cancellationToken);
}
