using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Application.Payments.Queries.GetPaymentsByOwner;

public sealed record PaymentResponse(
    Guid Id,
    // Shared response for both owner and vendor payments (see GetPaymentsByVendor) —
    // exactly one of OwnerId/VendorId is set, matching Payment's own shape.
    Guid? OwnerId,
    Guid? VendorId,
    decimal Amount,
    PaymentMethod PaymentMethod,
    DateTimeOffset PaymentDate,
    PaymentStatus Status,
    string? Notes,
    DateTimeOffset CreatedAt,
    Guid? ApartmentId,
    Guid? GarageId,
    // Resolved server-side from Payments' own Owner/Vendor/Apartment/Garage shadows
    // (see IPaymentDisplayEnricher) — null only when a shadow hasn't synced yet.
    string? PartyName = null,
    string? PropertyLabel = null);
