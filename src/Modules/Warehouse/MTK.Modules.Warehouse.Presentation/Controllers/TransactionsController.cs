using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Modules.Warehouse.Application.Transactions.Commands.RecordIssue;
using MTK.Modules.Warehouse.Application.Transactions.Queries.GetTransactionById;
using MTK.Modules.Warehouse.Application.Transactions.Queries.GetTransactionHistory;

namespace MTK.Modules.Warehouse.Presentation.Controllers;

public class TransactionsController(ISender sender) : BaseController(sender)
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetTransactionByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search(
        [FromBody] GetTransactionHistoryQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    // Mal qəbulu endpoint-i yoxdur: anbara giriş yalnız Payments modulundakı alışın
    // qəbulundan (GoodsReceivedIntegrationEvent) gəlir.

    [HttpPost("issue")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> RecordIssue(
        [FromBody] RecordIssueCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value, "Mal çıxarışı uğurla qeyd edildi")
            : BadRequest(result.Error);
    }
}
