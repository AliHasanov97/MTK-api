using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Transactions.Queries.SearchTransactions;

internal sealed class SearchTransactionsQueryHandler : IQueryHandler<SearchTransactionsQuery, SearchTransactionsResponse>
{
    private readonly ITransactionRepository _transactionRepository;

    public SearchTransactionsQueryHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<Result<SearchTransactionsResponse>> Handle(
        SearchTransactionsQuery request,
        CancellationToken cancellationToken)
    {
        var transactions = await _transactionRepository.SearchAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalCount = await _transactionRepository.CountAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            cancellationToken);

        var items = transactions.Select(t => new TransactionSearchResult(
            t.Id,
            t.Direction,
            t.Category,
            t.Amount,
            t.Description,
            t.TransactionDate,
            t.CreatedAt)).ToList();

        var response = new SearchTransactionsResponse(
            items,
            totalCount,
            request.Page ?? 1,
            request.PageSize ?? 10);

        return response;
    }
}
