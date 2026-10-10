using MTK.Common.Application.Authorization;

using MTK.Modules.Hr.Application.UnpaidLeaveOrders.DeleteUnpaidLeaveOrder;
using MTK.Modules.Hr.Application.UnpaidLeaveOrders.ExportUnpaidLeaveOrderPdf;
using MTK.Modules.Hr.Application.UnpaidLeaveOrders.GetUnpaidLeaveOrderById;
using MTK.Modules.Hr.Application.UnpaidLeaveOrders.SearchUnpaidLeaveOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MTK.Modules.Hr.Presentation.Controllers;

[RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
public class UnpaidLeaveOrdersController : BaseController
{
    public UnpaidLeaveOrdersController(ISender sender) : base(sender)
    {
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteUnpaidLeaveOrderCommand(id), cancellationToken);
        return result.IsSuccess ? Success() : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetUnpaidLeaveOrderByIdQuery(id), cancellationToken);
        return result.IsSuccess ? Success(result.Value) : NotFound(result.Error);
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search(
        [FromBody] SearchUnpaidLeaveOrdersQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Ödənişsiz məzuniyyət əmrini PDF formatında export edir
    /// </summary>
    [HttpGet("{id:guid}/export-pdf")]
    public async Task<IActionResult> ExportPdf(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ExportUnpaidLeaveOrderPdfQuery(id), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return File(result.Value.FileStream, "application/pdf", result.Value.FileName);
    }
}
