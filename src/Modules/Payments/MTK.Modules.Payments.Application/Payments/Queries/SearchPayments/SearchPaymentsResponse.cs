using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Application.Payments.Queries.SearchPayments;

public sealed record SearchPaymentsResponse(
    IReadOnlyCollection<PaymentSearchResult> Payments,
    int TotalCount,
    int Page,
    int PageSize);

public sealed record PaymentSearchResult(
    Guid Id,
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
