using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Contracts.Commands.RemoveContractGoodsItem;

public sealed record RemoveContractGoodsItemCommand(
    Guid ContractId,
    Guid GoodsItemId) : ICommand;
