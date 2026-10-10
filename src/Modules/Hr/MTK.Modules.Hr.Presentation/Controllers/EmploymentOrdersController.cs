using MTK.Common.Application.Authorization;

using MTK.Modules.Hr.Application.EmploymentOrders.DeleteEmploymentOrder;
using MTK.Modules.Hr.Application.EmploymentOrders.ExportEmploymentOrderPdf;
using MTK.Modules.Hr.Application.EmploymentOrders.GetEmploymentOrderById;
using MTK.Modules.Hr.Application.EmploymentOrders.SearchEmploymentOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MTK.Modules.Hr.Presentation.Controllers;

[RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
public class EmploymentOrdersController : BaseController
{
    public EmploymentOrdersController(ISender sender) : base(sender)
    {
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] SearchEmploymentOrdersQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteEmploymentOrderCommand(id), cancellationToken);
        return result.IsSuccess ? Success() : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetEmploymentOrderByIdQuery(id), cancellationToken);
        return result.IsSuccess ? Success(result.Value) : NotFound(result.Error);
    }

    [HttpGet("{id:guid}/export-pdf")]
    public async Task<IActionResult> ExportPdf([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ExportEmploymentOrderPdfQuery(id), cancellationToken);
        if (result.IsSuccess)
        {
            var response = result.Value;
            return File(response.FileStream, "application/pdf", response.FileName);
        }

        return NotFound(result.Error);
    }
}
