using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Rates;

namespace MTK.Modules.Payments.Application.Charges.Queries.SearchCharges;

public sealed record SearchChargesResponse(
    IReadOnlyCollection<ChargeSearchResult> Charges,
    int TotalCount,
    int Page,
    int PageSize);

public sealed record ChargeSearchResult(
    Guid Id,
    Guid OwnerId,
    Guid? ApartmentId,
    Guid? GarageId,
    string? Period,
    decimal Amount,
    decimal PaidAmount,
    ChargeStatus Status,
    DateTimeOffset CreatedAt,
    string? Description,
    decimal? AreaSquareMeters,
    decimal? RateAmount,
    RateType? RateType,
    // Borcun yaşı — FIFO sırasını və "avansdan ödənilib" işarəsini UI bununla qurur.
    DateTimeOffset IssuedOn,
    // Resolved server-side from Payments' own Owner/Vendor/Apartment/Garage shadows
    // (see IChargeDisplayEnricher) — null only when a shadow hasn't synced yet.
    string? PartyName = null,
    string? PropertyLabel = null);
