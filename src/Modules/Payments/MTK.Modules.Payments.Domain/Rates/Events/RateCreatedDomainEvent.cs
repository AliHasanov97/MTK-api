using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.Rates.Events;

public sealed record RateCreatedDomainEvent(
    Guid RateId,
    RateType RateType,
    decimal Amount) : DomainEvent;