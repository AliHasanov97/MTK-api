using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Rates;

namespace MTK.Modules.Payments.Application.Charges.Queries.GetChargesByOwner;

public sealed record ChargeResponse(
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
    // Snapshot of how Amount was calculated, so the UI can show its own math
    // instead of an opaque total (e.g. "130.20 m² × 0.36 ₼/m²").
    decimal? AreaSquareMeters,
    decimal? RateAmount,
    RateType? RateType,
    // Borcun yaşı — ödənişin avansdan gəlib-gəlmədiyini UI bununla ayırd edir.
    DateTimeOffset IssuedOn,
    // Resolved server-side from Payments' own Owner/Vendor/Apartment/Garage shadows
    // (see IChargeDisplayEnricher) — null only when a shadow hasn't synced yet.
    string? PartyName = null,
    string? PropertyLabel = null);
