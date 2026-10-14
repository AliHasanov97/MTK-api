using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.Customers.Events;

public sealed record CustomerCreatedDomainEvent(
    Guid CustomerId,
    string Name,
    CustomerType CustomerType) : DomainEvent;
