using MTK.Common.Application.Authorization;
using MTK.Common.Presentation.Responses;

using MTK.Modules.Hr.Application.Abstractions.Services.ExportService;
using MTK.Modules.Hr.Application.VacationReturnApplications.ConvertToOrder;
using MTK.Modules.Hr.Application.VacationReturnApplications.CreateVacationReturnApplication;
using MTK.Modules.Hr.Application.VacationReturnApplications.DeleteVacationReturnApplication;
using MTK.Modules.Hr.Application.VacationReturnApplications.GetVacationReturnApplicationById;
using MTK.Modules.Hr.Application.VacationReturnApplications.SearchVacationReturnApplications;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MTK.Modules.Hr.Presentation.Controllers;

[RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
public class VacationReturnApplicationsController : BaseController
{
    private readonly IExportService _exportService;

    public VacationReturnApplicationsController(ISender sender, IExportService exportService) : base(sender)
    {
        _exportService = exportService;
    }

    /// <summary>
    /// Yeni məzuniyyətdən geri qayıtma ərizəsi yaradır
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateVacationReturnApplicationCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// ID-yə görə məzuniyyətdən geri qayıtma ərizəsini götür
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetVacationReturnApplicationByIdQuery(id), cancellationToken);
        return result.IsSuccess ? Success(result.Value) : NotFound(result.Error);
    }

    /// <summary>
    /// Məzuniyyətdən geri qayıtma ərizələrini axtar (pagination ilə)
    /// </summary>
    [HttpPost("search")]
    public async Task<IActionResult> Search(
        [FromBody] SearchVacationReturnApplicationsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Məzuniyyətdən geri qayıtma ərizəsini sil (yalnız PendingApproval statusunda)
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteVacationReturnApplicationCommand(id), cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    /// <summary>
    /// Məzuniyyətdən geri qayıtma ərizəsini PDF formatında export et
    /// </summary>
    [HttpGet("{id:guid}/export-pdf")]
    public async Task<IActionResult> ExportPdf(Guid id, CancellationToken cancellationToken)
    {
        var result = await _exportService.ExportVacationReturnApplicationToPdfAsync(id, cancellationToken);

        using var memoryStream = new MemoryStream();
        await result.FileStream.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        return File(
            memoryStream.ToArray(),
            "application/pdf",
            result.FileName);
    }

    /// <summary>
    /// VacationReturnApplication-ı VacationReturnOrder-ə çevirir
    /// </summary>
    [HttpPost("{id:guid}/convert-to-order")]
    public async Task<IActionResult> ConvertToOrder(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new ConvertVacationReturnToOrderCommand(id),
            cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }
}
