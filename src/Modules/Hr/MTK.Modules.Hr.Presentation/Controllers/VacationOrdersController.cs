using MTK.Modules.Hr.Application.VacationOrders.ExportVacationOrderPdf;
using MTK.Common.Application.Authorization;

using MTK.Modules.Hr.Application.VacationOrders.GetVacationOrderById;
using MTK.Modules.Hr.Application.VacationOrders.SearchVacationOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MTK.Modules.Hr.Presentation.Controllers;

[RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
public class VacationOrdersController : BaseController
{
    public VacationOrdersController(ISender sender) : base(sender)
    {
    }

    /// <summary>
    /// ID-yə görə ödənişli məzuniyyət əmrini götür
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetVacationOrderByIdQuery(id), cancellationToken);
        return result.IsSuccess ? Success(result.Value) : NotFound(result.Error);
    }

    /// <summary>
    /// Ödənişli məzuniyyət əmrlərini axtar (pagination ilə)
    /// </summary>
    [HttpPost("search")]
    public async Task<IActionResult> Search(
        [FromBody] SearchVacationOrdersQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Əmək məzuniyyəti əmrini PDF formatında export edir
    /// </summary>
    [HttpGet("{id:guid}/export-pdf")]
    public async Task<IActionResult> ExportPdf(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ExportVacationOrderPdfQuery(id), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return File(result.Value.FileStream, "application/pdf", result.Value.FileName);
    }
}
