using MTK.Modules.Payments.Domain.Charges;

namespace MTK.Modules.Payments.Application.VendorCharges.Queries.SearchVendorCharges;

public sealed record VendorChargeResponse(
    Guid Id,
    Guid? ContractId,
    Guid VendorId,
    Guid? ContractServiceId,
    string? Period,
    string? Description,
    decimal Amount,
    decimal PaidAmount,
    decimal OutstandingAmount,
    ChargeStatus Status,
    DateTimeOffset ChargeDate,
    DateTimeOffset? DueDate,
    bool IsOverdue,
    DateTimeOffset CreatedAt);

public sealed record SearchVendorChargesResponse(
    IReadOnlyList<VendorChargeResponse> Items,
    int TotalCount,
    int Page,
    int PageSize);
