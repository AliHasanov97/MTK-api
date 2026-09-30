using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Domain.Contracts;

namespace MTK.Modules.Payments.Application.Contracts.Commands.AddContractGoodsItem;

/// <summary>
/// Müqaviləyə mal sətri əlavə edir (məs. "Sement M500", torba, 12 AZN).
/// Mal sətri özü borc yaratmır — borc konkret tədarük (qaimə) üzrə yaranır.
/// Yalnız Draft mərhələsində mümkündür.
/// </summary>
public sealed record AddContractGoodsItemCommand(
    Guid ContractId,
    string Name,
    string Unit,
    decimal UnitPrice,
    decimal? AgreedQuantity = null,
    int? PaymentTermDays = null,
    string? Description = null) : ICommand<Guid>;
