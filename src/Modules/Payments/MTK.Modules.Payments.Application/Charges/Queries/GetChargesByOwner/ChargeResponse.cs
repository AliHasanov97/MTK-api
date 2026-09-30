using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Rates;

namespace MTK.Modules.Payments.Application.Charges.Queries.GetChargesByOwner;

public sealed record ChargeResponse(
    Guid Id,
    Guid OwnerId,
    PropertyType? PropertyType,
    Guid? PropertyId,
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
    DateTimeOffset IssuedOn);
