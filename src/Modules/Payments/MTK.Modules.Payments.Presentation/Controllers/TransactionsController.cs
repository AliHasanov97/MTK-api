using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Common.Domain.Queries;
using MTK.Modules.Payments.Application.Transactions.Commands.CreateManualTransaction;
using MTK.Modules.Payments.Application.Transactions.Queries.GetTransactionsSummary;
using MTK.Modules.Payments.Application.Transactions.Queries.SearchTransactions;
using MTK.Modules.Payments.Domain.Transactions;

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

    // Ledger entries are append-only and, for a resident/vendor payment, written
    // only by the payment flow — there is no endpoint to create or delete *that*
    // kind by hand, so the journal always matches the payments it reflects. A
    // manual entry (an ad-hoc expense or income with no charge/vendor behind it,
    // e.g. a utility bill paid directly) is the one deliberate exception below.
    [HttpPost]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> CreateManualTransaction(
        [FromBody] CreateManualTransactionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateManualTransactionCommand(
            request.Direction,
            request.Category,
            request.Amount,
            request.Description);

        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value, "Əməliyyat uğurla qeydə alındı")
            : BadRequest(result.Error);
    }
}

public sealed record SearchTransactionsRequest(
    List<QueryFilter>? Filters,
    SortCriteria? SortCriteria,
    string? SearchTerm,
    int? Page,
    int? PageSize);

public sealed record TransactionsSummaryRequest(List<QueryFilter>? Filters);

public sealed record CreateManualTransactionRequest(
    TransactionDirection Direction,
    string Category,
    decimal Amount,
    string? Description);
