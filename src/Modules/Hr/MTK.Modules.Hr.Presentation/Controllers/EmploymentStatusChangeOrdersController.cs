using MTK.Common.Application.Authorization;

using MTK.Modules.Hr.Application.EmploymentStatusChangeOrders.DeleteEmploymentStatusChangeOrder;
using MTK.Modules.Hr.Application.EmploymentStatusChangeOrders.ExportEmploymentStatusChangeOrderPdf;
using MTK.Modules.Hr.Application.EmploymentStatusChangeOrders.GetEmploymentStatusChangeOrderById;
using MTK.Modules.Hr.Application.EmploymentStatusChangeOrders.SearchEmploymentStatusChangeOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MTK.Modules.Hr.Presentation.Controllers;

[RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
public class EmploymentStatusChangeOrdersController : BaseController
{
    public EmploymentStatusChangeOrdersController(ISender sender) : base(sender)
    {
    }

    /// <summary>
    /// İş rejimi dəyişikliyi əmrini silir
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteEmploymentStatusChangeOrderCommand(id), cancellationToken);
        return result.IsSuccess ? Success() : BadRequest(result.Error);
    }

    /// <summary>
    /// İş rejimi dəyişikliyi əmrini Id ilə gətirir
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetEmploymentStatusChangeOrderByIdQuery(id), cancellationToken);
        return result.IsSuccess ? Success(result.Value) : NotFound(result.Error);
    }

    /// <summary>
    /// İş rejimi dəyişikliyi əmrlərini axtarır
    /// </summary>
    [HttpPost("search")]
    public async Task<IActionResult> Search(
        [FromBody] SearchEmploymentStatusChangeOrdersQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// İş rejimi dəyişikliyi əmrini PDF formatında export edir
    /// </summary>
    [HttpGet("{id:guid}/export-pdf")]
    public async Task<IActionResult> ExportPdf(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new ExportEmploymentStatusChangeOrderPdfQuery(id),
            cancellationToken);

        if (result.IsSuccess)
        {
            var response = result.Value;
            return File(response.FileStream, "application/pdf", response.FileName);
        }

        return NotFound(result.Error);
    }
}
