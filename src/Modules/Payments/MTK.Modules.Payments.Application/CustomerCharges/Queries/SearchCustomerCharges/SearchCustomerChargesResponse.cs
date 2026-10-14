using MTK.Modules.Payments.Domain.Charges;

namespace MTK.Modules.Payments.Application.CustomerCharges.Queries.SearchCustomerCharges;

public sealed record CustomerChargeResponse(
    Guid Id,
    Guid? ContractId,
    Guid CustomerId,
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

public sealed record SearchCustomerChargesResponse(
    IReadOnlyList<CustomerChargeResponse> Items,
    int TotalCount,
    int Page,
    int PageSize);
