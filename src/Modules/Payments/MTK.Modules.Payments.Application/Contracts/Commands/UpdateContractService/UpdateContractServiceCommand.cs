using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Domain.Contracts;

namespace MTK.Modules.Payments.Application.Contracts.Commands.UpdateContractService;

public sealed record UpdateContractServiceCommand(
    Guid ContractId,
    Guid ServiceId,
    string Name,
    decimal UnitPrice,
    BillingPeriod BillingPeriod,
    decimal Quantity,
    string Unit = "ay",
    string? Description = null,
    DateTimeOffset? ServiceStartDate = null,
    DateTimeOffset? ServiceEndDate = null,
    int? PaymentTermDays = null) : ICommand;
