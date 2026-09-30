using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Contracts.Commands.UpdateContractGoodsItem;

public sealed record UpdateContractGoodsItemCommand(
    Guid ContractId,
    Guid GoodsItemId,
    string Name,
    string Unit,
    decimal UnitPrice,
    decimal? AgreedQuantity = null,
    int? PaymentTermDays = null,
    string? Description = null) : ICommand;
