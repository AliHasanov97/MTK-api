namespace MTK.Common.Application.EventBus;

public abstract class IntegrationEvent : IIntegrationEvent
{
    protected IntegrationEvent()
    {
        IntegrationEventId = Guid.NewGuid();
        OccurredOnUtc = DateTime.UtcNow;
    }

    protected IntegrationEvent(Guid id, DateTime occurredOnUtc)
    {
        IntegrationEventId = id;
        OccurredOnUtc = occurredOnUtc;
    }

    public Guid IntegrationEventId { get; init; }
    public DateTime OccurredOnUtc { get; init; }
}
