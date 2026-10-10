using MTK.Common.Application.Authorization;

using MTK.Modules.Hr.Application.CompensationOrders.DeleteCompensationOrder;
using MTK.Modules.Hr.Application.CompensationOrders.ExportCompensationOrderPdf;
using MTK.Modules.Hr.Application.CompensationOrders.GetCompensationOrderById;
using MTK.Modules.Hr.Application.CompensationOrders.SearchCompensationOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MTK.Modules.Hr.Presentation.Controllers;

[RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
public class CompensationOrdersController : BaseController
{
    public CompensationOrdersController(ISender sender) : base(sender)
    {
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search(
        [FromBody] SearchCompensationOrdersQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetCompensationOrderByIdQuery(id), cancellationToken);
        return result.IsSuccess ? Success(result.Value) : NotFound(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteCompensationOrderCommand(id), cancellationToken);
        return result.IsSuccess ? Success() : BadRequest(result.Error);
    }

    /// <summary>
    /// CompensationOrder-i PDF-ə export edir
    /// </summary>
    [HttpGet("{id:guid}/export-pdf")]
    public async Task<IActionResult> ExportPdf(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new ExportCompensationOrderPdfQuery(id),
            cancellationToken);

        if (!result.IsSuccess)
            return NotFound(result.Error);

        return File(result.Value.FileStream, "application/pdf", result.Value.FileName);
    }
}