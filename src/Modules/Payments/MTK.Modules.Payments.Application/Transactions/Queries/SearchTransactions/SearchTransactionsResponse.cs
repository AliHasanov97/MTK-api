using MTK.Modules.Payments.Domain.Transactions;

namespace MTK.Modules.Payments.Application.Transactions.Queries.SearchTransactions;

public sealed record SearchTransactionsResponse(
    IReadOnlyCollection<TransactionSearchResult> Transactions,
    int TotalCount,
    int Page,
    int PageSize);

public sealed record TransactionSearchResult(
    Guid Id,
    TransactionDirection Direction,
    string Category,
    decimal Amount,
    string? Description,
    DateTimeOffset TransactionDate,
    DateTimeOffset CreatedAt,
    Guid? SourcePaymentId,
    Guid? SourcePurchaseId,
    string DocumentType,
    Guid ReferenceId);
