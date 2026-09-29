using MTK.Modules.Payments.Domain.PropertyOwnerships;
using MTK.Modules.Payments.Domain.Rates;

namespace MTK.Modules.Payments.Application.Rates.Queries.GetCurrentRates;

public sealed record RateResponse(
    Guid Id,
    RateType RateType,
    decimal Amount,
    DateTimeOffset EffectiveFrom,
    DateTimeOffset? EffectiveTo,
    string? Description,
    GarageType? GarageType);
