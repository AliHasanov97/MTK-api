using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Rates.Commands.UpdateRate;

public sealed record UpdateRateCommand(
    Guid RateId,
    decimal Amount,
    string? Description) : ICommand;
