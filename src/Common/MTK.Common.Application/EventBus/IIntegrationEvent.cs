namespace MTK.Common.Application.EventBus;

public interface IIntegrationEvent
{
    Guid IntegrationEventId { get; }
    DateTime OccurredOnUtc { get; }
}
