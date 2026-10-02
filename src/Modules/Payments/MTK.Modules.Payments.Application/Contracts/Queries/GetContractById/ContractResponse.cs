using MTK.Modules.Payments.Domain.Contracts;

namespace MTK.Modules.Payments.Application.Contracts.Queries.GetContractById;

public sealed record ContractResponse(
    Guid Id,
    string Number,
    Guid VendorId,
    string? VendorName,
    Guid? CreatedByUserId,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    ContractStatus Status,
    string? Note,
    bool IsExpired,
    bool IsActive,
    decimal MonthlyAmount,
    decimal TotalAmount,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    IReadOnlyCollection<ContractServiceResponse> Services);

public sealed record ContractServiceResponse(
    Guid Id,
    string Name,
    string? Description,
    decimal UnitPrice,
    BillingPeriod BillingPeriod,
    decimal PeriodAmount,
    DateTimeOffset? ServiceStartDate,
    DateTimeOffset? ServiceEndDate,
    int? PaymentTermDays,
    bool IsActive);
