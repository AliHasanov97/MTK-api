using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Modules.Payments.Application.Transactions.Commands.CreateManualTransaction;
using MTK.Modules.Payments.Application.Transactions.Queries.GetTransactionsSummary;
using MTK.Modules.Payments.Application.Transactions.Queries.SearchTransactions;

namespace MTK.Modules.Payments.Presentation.Controllers;

public class TransactionsController(ISender sender) : BaseController(sender)
{
    [HttpPost("search")]
    public async Task<IActionResult> SearchTransactions(
        [FromBody] SearchTransactionsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpPost("summary")]
    public async Task<IActionResult> GetTransactionsSummary(
        [FromBody] GetTransactionsSummaryQuery query,
        CancellationToken cancellationToken)
    {
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
        [FromBody] CreateManualTransactionCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value, "Əməliyyat uğurla qeydə alındı")
            : BadRequest(result.Error);
    }
}
