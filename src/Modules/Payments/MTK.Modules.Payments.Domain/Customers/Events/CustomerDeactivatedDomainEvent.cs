using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.Customers.Events;

public sealed record CustomerDeactivatedDomainEvent(
    Guid CustomerId) : DomainEvent;
