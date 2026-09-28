using MTK.Common.Application.EventBus;

namespace MTK.Modules.Payments.IntegrationEvents.Payments;

public sealed class PaymentReceivedIntegrationEvent : IntegrationEvent
{
    public PaymentReceivedIntegrationEvent(
        Guid integrationEventId,
        DateTime occurredOnUtc,
        Guid paymentId,
        Guid ownerId,
        decimal amount)
        : base(integrationEventId, occurredOnUtc)
    {
        PaymentId = paymentId;
        OwnerId = ownerId;
        Amount = amount;
    }

    public Guid PaymentId { get; }
    public Guid OwnerId { get; }
    public decimal Amount { get; }
}
