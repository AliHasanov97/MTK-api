using MTK.Modules.Payments.Domain.VendorCharges;

namespace MTK.Modules.Payments.Application.VendorCharges.Queries.SearchVendorCharges;

public sealed record VendorChargeResponse(
    Guid Id,
    Guid ContractId,
    Guid VendorId,
    Guid? ContractServiceId,
    Guid? ContractGoodsItemId,
    string? Period,
    string Description,
    string? Reference,
    decimal UnitPrice,
    decimal Quantity,
    decimal Amount,
    decimal PaidAmount,
    decimal OutstandingAmount,
    VendorChargeStatus Status,
    VendorChargeSource Source,
    DateTimeOffset ChargeDate,
    DateTimeOffset? DueDate,
    string Currency,
    string? CancellationReason,
    bool IsOverdue,
    DateTimeOffset CreatedAt);

public sealed record SearchVendorChargesResponse(
    IReadOnlyList<VendorChargeResponse> Items,
    int TotalCount,
    int Page,
    int PageSize);
