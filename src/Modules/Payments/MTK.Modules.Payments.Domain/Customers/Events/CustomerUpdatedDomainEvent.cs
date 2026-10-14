using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.Customers.Events;

public sealed record CustomerUpdatedDomainEvent(
    Guid CustomerId,
    string Name) : DomainEvent;
