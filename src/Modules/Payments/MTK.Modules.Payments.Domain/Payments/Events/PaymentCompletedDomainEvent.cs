using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.Payments.Events;

public sealed record PaymentCompletedDomainEvent(
    Guid PaymentId,
    Guid OwnerId,
    decimal Amount) : DomainEvent;
