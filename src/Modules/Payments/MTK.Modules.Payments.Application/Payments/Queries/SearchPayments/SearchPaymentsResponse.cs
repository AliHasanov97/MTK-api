using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Application.Payments.Queries.SearchPayments;

public sealed record SearchPaymentsResponse(
    IReadOnlyCollection<PaymentSearchResult> Payments,
    int TotalCount,
    int Page,
    int PageSize);

public sealed record PaymentSearchResult(
    Guid Id,
    Guid OwnerId,
    decimal Amount,
    PaymentMethod PaymentMethod,
    DateTimeOffset PaymentDate,
    PaymentStatus Status,
    string? Reference,
    string? Notes,
    DateTimeOffset CreatedAt);
