using MTK.Common.Application.Authorization;

using MTK.Modules.Hr.Application.Abstractions.Services.ExportService;
using MTK.Modules.Hr.Application.VacationReturnOrders.GetVacationReturnOrderById;
using MTK.Modules.Hr.Application.VacationReturnOrders.SearchVacationReturnOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MTK.Modules.Hr.Presentation.Controllers;

[RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
public class VacationReturnOrdersController : BaseController
{
    private readonly IExportService _exportService;

    public VacationReturnOrdersController(ISender sender, IExportService exportService) : base(sender)
    {
        _exportService = exportService;
    }

    /// <summary>
    /// ID-yə görə məzuniyyətdən geri qayıtma əmrini götür
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetVacationReturnOrderByIdQuery(id), cancellationToken);
        return result.IsSuccess ? Success(result.Value) : NotFound(result.Error);
    }

    /// <summary>
    /// Məzuniyyətdən geri qayıtma əmrlərini axtar (pagination ilə)
    /// </summary>
    [HttpPost("search")]
    public async Task<IActionResult> Search(
        [FromBody] SearchVacationReturnOrdersQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Məzuniyyətdən geri qayıtma əmrini PDF formatında export et
    /// </summary>
    [HttpGet("{id:guid}/export-pdf")]
    public async Task<IActionResult> ExportPdf(Guid id, CancellationToken cancellationToken)
    {
        var result = await _exportService.ExportVacationReturnOrderToPdfAsync(id, cancellationToken);

        using var memoryStream = new MemoryStream();
        await result.FileStream.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        return File(
            memoryStream.ToArray(),
            "application/pdf",
            result.FileName);
    }
}
