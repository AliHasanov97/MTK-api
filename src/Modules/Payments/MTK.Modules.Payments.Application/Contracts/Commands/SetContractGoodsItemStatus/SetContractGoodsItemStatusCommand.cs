using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Contracts.Commands.SetContractGoodsItemStatus;

/// <summary>
/// Mal sətrini dayandırır/bərpa edir — müqavilə aktiv qalır, sadəcə dayandırılmış
/// sətir üzrə yeni tədarük borcu yaranmır.
/// </summary>
public sealed record SetContractGoodsItemStatusCommand(
    Guid ContractId,
    Guid GoodsItemId,
    bool IsActive) : ICommand;
