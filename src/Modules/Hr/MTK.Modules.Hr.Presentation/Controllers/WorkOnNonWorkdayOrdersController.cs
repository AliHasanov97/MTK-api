using MTK.Common.Application.Authorization;

using MTK.Modules.Hr.Application.WorkOnNonWorkdayOrders.AddWorkOnNonWorkdayOrder;
using MTK.Modules.Hr.Application.WorkOnNonWorkdayOrders.DeleteWorkOnNonWorkdayOrder;
using MTK.Modules.Hr.Application.WorkOnNonWorkdayOrders.ExportWorkOnNonWorkdayOrderPdf;
using MTK.Modules.Hr.Application.WorkOnNonWorkdayOrders.GetWorkOnNonWorkdayOrderById;
using MTK.Modules.Hr.Application.WorkOnNonWorkdayOrders.SearchWorkOnNonWorkdayOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MTK.Modules.Hr.Presentation.Controllers;

[RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
public class WorkOnNonWorkdayOrdersController : BaseController
{
    public WorkOnNonWorkdayOrdersController(ISender sender) : base(sender)
    {
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddWorkOnNonWorkdayOrderCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteWorkOnNonWorkdayOrderCommand { Id = id }, cancellationToken);
        return result.IsSuccess ? Success() : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetWorkOnNonWorkdayOrderByIdQuery { Id = id }, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : NotFound(result.Error);
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] SearchWorkOnNonWorkdayOrdersQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}/export-pdf")]
    public async Task<IActionResult> ExportPdf([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new ExportWorkOnNonWorkdayOrderPdfQuery(id),
            cancellationToken);

        if (result.IsSuccess)
        {
            var response = result.Value;
            return File(response.FileStream, "application/pdf", response.FileName);
        }

        return NotFound(result.Error);
    }
}
