using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Domain.Queries;
using MTK.Modules.Payments.Application.Transactions.Queries.GetTransactionsSummary;
using MTK.Modules.Payments.Application.Transactions.Queries.SearchTransactions;

namespace MTK.Modules.Payments.Presentation.Controllers;

public class TransactionsController(ISender sender) : BaseController(sender)
{
    [HttpPost("search")]
    public async Task<IActionResult> SearchTransactions(
        [FromBody] SearchTransactionsRequest request,
        CancellationToken cancellationToken)
    {
        var query = new SearchTransactionsQuery(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            request.Page,
            request.PageSize);

        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpPost("summary")]
    public async Task<IActionResult> GetTransactionsSummary(
        [FromBody] TransactionsSummaryRequest request,
        CancellationToken cancellationToken)
    {
        var query = new GetTransactionsSummaryQuery(request.Filters);
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    // Ledger entries are append-only: a transaction is written by the payment flow
    // (or by its reversal) and there is deliberately no endpoint to create or
    // delete one by hand, so the journal always matches the payments it reflects.
}

public sealed record SearchTransactionsRequest(
    List<QueryFilter>? Filters,
    SortCriteria? SortCriteria,
    string? SearchTerm,
    int? Page,
    int? PageSize);

public sealed record TransactionsSummaryRequest(List<QueryFilter>? Filters);
