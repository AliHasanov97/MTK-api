using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Payments.Application.Transactions.Queries.GetTransactionsSummary;

public sealed record GetTransactionsSummaryQuery(List<QueryFilter>? Filters) : IQuery<TransactionsSummaryResponse>;

public sealed record TransactionsSummaryResponse(decimal TotalIncome, decimal TotalExpense, decimal Net);
