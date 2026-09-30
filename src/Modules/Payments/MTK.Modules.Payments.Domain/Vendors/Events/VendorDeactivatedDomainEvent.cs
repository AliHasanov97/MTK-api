using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.Vendors.Events;

public sealed record VendorDeactivatedDomainEvent(
    Guid VendorId) : DomainEvent;
