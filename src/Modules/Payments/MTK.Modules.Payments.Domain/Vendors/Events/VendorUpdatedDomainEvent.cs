using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.Vendors.Events;

public sealed record VendorUpdatedDomainEvent(
    Guid VendorId,
    string Name) : DomainEvent;
