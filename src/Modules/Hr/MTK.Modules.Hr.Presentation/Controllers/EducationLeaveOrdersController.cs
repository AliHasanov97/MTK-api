using MTK.Common.Application.Authorization;

using MTK.Modules.Hr.Application.EducationLeaveOrders.DeleteEducationLeaveOrder;
using MTK.Modules.Hr.Application.EducationLeaveOrders.ExportEducationLeaveOrderPdf;
using MTK.Modules.Hr.Application.EducationLeaveOrders.GetEducationLeaveOrderById;
using MTK.Modules.Hr.Application.EducationLeaveOrders.SearchEducationLeaveOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MTK.Modules.Hr.Presentation.Controllers;

[RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
public class EducationLeaveOrdersController : BaseController
{
    public EducationLeaveOrdersController(ISender sender) : base(sender)
    {
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteEducationLeaveOrderCommand(id), cancellationToken);
        return result.IsSuccess ? Success() : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetEducationLeaveOrderByIdQuery(id), cancellationToken);
        return result.IsSuccess ? Success(result.Value) : NotFound(result.Error);
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search(
        [FromBody] SearchEducationLeaveOrdersQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Təhsil məzuniyyəti əmrini PDF formatında export edir
    /// </summary>
    [HttpGet("{id:guid}/export-pdf")]
    public async Task<IActionResult> ExportPdf(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ExportEducationLeaveOrderPdfQuery(id), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return File(result.Value.FileStream, "application/pdf", result.Value.FileName);
    }
}
