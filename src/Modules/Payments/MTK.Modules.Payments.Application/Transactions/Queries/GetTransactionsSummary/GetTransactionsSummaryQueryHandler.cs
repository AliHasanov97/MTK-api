using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Domain.Transactions;

namespace MTK.Modules.Payments.Application.Transactions.Queries.GetTransactionsSummary;

internal sealed class GetTransactionsSummaryQueryHandler
    : IQueryHandler<GetTransactionsSummaryQuery, TransactionsSummaryResponse>
{
    private readonly ITransactionRepository _transactionRepository;

    public GetTransactionsSummaryQueryHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<Result<TransactionsSummaryResponse>> Handle(
        GetTransactionsSummaryQuery request,
        CancellationToken cancellationToken)
    {
        // Unbounded (SearchAsync's default page cap is 100) — this is a totals
        // rollup over whatever the filters match, not a page of rows to render.
        var transactions = await _transactionRepository.SearchAsync(
            request.Filters, null, null, 0, 100_000, cancellationToken);

        decimal totalIncome = transactions.Where(t => t.Direction == TransactionDirection.Income).Sum(t => t.Amount);
        decimal totalExpense = transactions.Where(t => t.Direction == TransactionDirection.Expense).Sum(t => t.Amount);

        return new TransactionsSummaryResponse(totalIncome, totalExpense, totalIncome - totalExpense);
    }
}
