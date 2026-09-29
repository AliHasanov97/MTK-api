using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Domain.PropertyOwnerships;
using MTK.Modules.Payments.Domain.Rates;

namespace MTK.Modules.Payments.Application.Rates.Commands.CreateRate;

public sealed record CreateRateCommand(
    RateType RateType,
    decimal Amount,
    DateTimeOffset EffectiveFrom,
    string? Description,
    // Only meaningful when RateType == FixedGarage. Null = default rate for any garage
    // type that doesn't have a more specific one configured.
    GarageType? GarageType = null) : ICommand<Guid>;
