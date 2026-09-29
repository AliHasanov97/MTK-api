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
    PropertyType PropertyType,
    Guid PropertyId,
    string Period,
    decimal Amount,
    decimal PaidAmount,
    ChargeStatus Status,
    DateTimeOffset CreatedAt,
    string? Description,
    decimal? AreaSquareMeters,
    decimal RateAmount,
    RateType RateType);
