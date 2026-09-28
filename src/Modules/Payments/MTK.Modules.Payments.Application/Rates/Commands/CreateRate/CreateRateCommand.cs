using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Domain.Rates;

namespace MTK.Modules.Payments.Application.Rates.Commands.CreateRate;

public sealed record CreateRateCommand(
    RateType RateType,
    decimal Amount,
    DateTimeOffset EffectiveFrom,
    string? Description) : ICommand<Guid>;
