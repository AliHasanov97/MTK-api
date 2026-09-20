using MassTransit;
using Microsoft.Extensions.Logging;
using MTK.Common.Application.EventBus;

namespace MTK.Common.Infrastructure.EventBus;

internal sealed class EventBus(IBus bus, ILogger<EventBus> logger) : IEventBus
{
    public async Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken = default)
        where T : IIntegrationEvent
    {
        await bus.Publish(integrationEvent, cancellationToken);

        logger.LogInformation(
            "Published integration event {EventType} with ID {EventId}",
            typeof(T).Name,
            integrationEvent.IntegrationEventId);
    }
}
