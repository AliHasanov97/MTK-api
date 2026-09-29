using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.Payments.Events;

public sealed record PaymentCancelledDomainEvent(
    Guid PaymentId,
    Guid OwnerId,
    decimal Amount) : DomainEvent;
