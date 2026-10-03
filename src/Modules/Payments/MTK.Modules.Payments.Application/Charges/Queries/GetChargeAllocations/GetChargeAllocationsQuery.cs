using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Charges.Queries.GetChargeAllocations;

public sealed record GetChargeAllocationsQuery(Guid ChargeId) : IQuery<IReadOnlyCollection<ChargeAllocationResponse>>;

public sealed record ChargeAllocationResponse(
    // PaymentAllocation-un öz Id-si — AuditLogs-da "kim icra edib"-i axtarmaq üçün.
    Guid Id,
    Guid PaymentId,
    decimal AllocatedAmount,
    DateTimeOffset PaymentDate,
    string PaymentStatus,
    // Bu haqqın öz qalıq borcu bu konkret paylanma tətbiq olunandan dərhal sonra.
    decimal RemainingDebtAfterPayment,
    // true — əvvəlki avansdan bağlanıb; false — elə bu ödənişin özündən birbaşa.
    bool IsFromAdvance);
