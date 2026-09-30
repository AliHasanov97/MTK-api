using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.Vendors.Events;

public sealed record VendorCreatedDomainEvent(
    Guid VendorId,
    string Name,
    VendorType VendorType) : DomainEvent;
