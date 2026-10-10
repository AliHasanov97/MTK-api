using MTK.Common.Application.Authorization;

using MTK.Modules.Hr.Application.OrdersForChangeOfPosition.DeleteOrderForChangeOfPosition;
using MTK.Modules.Hr.Application.OrdersForChangeOfPosition.ExportOrderForChangeOfPositionPdf;
using MTK.Modules.Hr.Application.OrdersForChangeOfPosition.GetOrderForChangeOfPositionById;
using MTK.Modules.Hr.Application.OrdersForChangeOfPosition.SearchOrdersForChangeOfPosition;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MTK.Modules.Hr.Presentation.Controllers;

[RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
public class OrdersForChangeOfPositionController : BaseController
{
    public OrdersForChangeOfPositionController(ISender sender) : base(sender)
    {
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] SearchOrdersForChangeOfPositionQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteOrderForChangeOfPositionCommand { Id = id }, cancellationToken);
        return result.IsSuccess ? Success() : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetOrderForChangeOfPositionByIdQuery { Id = id }, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : NotFound(result.Error);
    }

    [HttpGet("{id:guid}/export-pdf")]
    public async Task<IActionResult> ExportPdf([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new ExportOrderForChangeOfPositionPdfQuery(id),
            cancellationToken);

        if (result.IsSuccess)
        {
            var response = result.Value;
            return File(response.FileStream, "application/pdf", response.FileName);
        }

        return NotFound(result.Error);
    }
}
