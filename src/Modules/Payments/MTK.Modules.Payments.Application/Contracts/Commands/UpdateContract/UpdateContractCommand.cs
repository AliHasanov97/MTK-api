using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Contracts.Commands.UpdateContract;

public sealed record UpdateContractCommand(
    Guid ContractId,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    string? Note = null) : ICommand;
