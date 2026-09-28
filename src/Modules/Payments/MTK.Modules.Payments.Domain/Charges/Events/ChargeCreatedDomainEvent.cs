using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.Charges.Events;

public sealed record ChargeCreatedDomainEvent(
    Guid ChargeId,
    Guid OwnerId,
    decimal Amount,
    string Period) : DomainEvent;
