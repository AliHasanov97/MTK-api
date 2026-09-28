using MTK.Common.Application.EventBus;

namespace MTK.Modules.Payments.IntegrationEvents.Charges;

public sealed class ChargeCreatedIntegrationEvent : IntegrationEvent
{
    public ChargeCreatedIntegrationEvent(
        Guid integrationEventId,
        DateTime occurredOnUtc,
        Guid chargeId,
        Guid ownerId,
        decimal amount,
        string period)
        : base(integrationEventId, occurredOnUtc)
    {
        ChargeId = chargeId;
        OwnerId = ownerId;
        Amount = amount;
        Period = period;
    }

    public Guid ChargeId { get; }
    public Guid OwnerId { get; }
    public decimal Amount { get; }
    public string Period { get; }
}
