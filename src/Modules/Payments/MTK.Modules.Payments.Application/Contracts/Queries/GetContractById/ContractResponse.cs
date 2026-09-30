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
    string Currency,
    string? Note,
    bool IsExpired,
    bool IsActive,
    decimal MonthlyAmount,
    decimal TotalAmount,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    IReadOnlyCollection<ContractServiceResponse> Services,
    IReadOnlyCollection<ContractGoodsItemResponse> GoodsItems);

public sealed record ContractServiceResponse(
    Guid Id,
    string Name,
    string? Description,
    string Unit,
    decimal UnitPrice,
    decimal Quantity,
    BillingPeriod BillingPeriod,
    decimal PeriodAmount,
    DateTimeOffset? ServiceStartDate,
    DateTimeOffset? ServiceEndDate,
    int? PaymentTermDays,
    bool IsActive);

public sealed record ContractGoodsItemResponse(
    Guid Id,
    string Name,
    string? Description,
    string Unit,
    decimal UnitPrice,
    decimal? AgreedQuantity,
    int? PaymentTermDays,
    bool IsActive);
