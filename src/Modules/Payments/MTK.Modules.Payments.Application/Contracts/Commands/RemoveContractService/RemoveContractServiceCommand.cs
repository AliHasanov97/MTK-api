using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Contracts.Commands.RemoveContractService;

public sealed record RemoveContractServiceCommand(
    Guid ContractId,
    Guid ServiceId) : ICommand;
