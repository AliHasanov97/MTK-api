using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Domain.Contracts;

namespace MTK.Modules.Payments.Application.Contracts.Commands.AddContractService;

/// <summary>
/// Müqaviləyə xidmət əlavə edir (məs. "Liftə aylıq texniki xidmət", 50 AZN, aylıq).
/// Yalnız Draft mərhələsində mümkündür.
/// </summary>
public sealed record AddContractServiceCommand(
    Guid ContractId,
    string Name,
    decimal UnitPrice,
    BillingPeriod BillingPeriod = BillingPeriod.Monthly,
    decimal Quantity = 1,
    string Unit = "ay",
    string? Description = null,
    DateTimeOffset? ServiceStartDate = null,
    DateTimeOffset? ServiceEndDate = null,
    int? PaymentTermDays = null) : ICommand<Guid>;
