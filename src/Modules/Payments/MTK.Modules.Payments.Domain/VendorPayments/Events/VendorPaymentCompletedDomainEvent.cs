using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.VendorPayments.Events;

public sealed record VendorPaymentCompletedDomainEvent(
    Guid VendorPaymentId,
    Guid VendorChargeId,
    Guid VendorId,
    decimal Amount) : DomainEvent;
