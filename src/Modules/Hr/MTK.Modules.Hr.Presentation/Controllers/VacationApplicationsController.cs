using MTK.Common.Application.Authorization;
using MTK.Common.Presentation.Responses;

using MTK.Modules.Hr.Application.VacationApplications.ConvertToOrder;
using MTK.Modules.Hr.Application.VacationApplications.CreateVacationApplication;
using MTK.Modules.Hr.Application.VacationApplications.DeleteVacationApplication;
using MTK.Modules.Hr.Application.VacationApplications.ExportVacationApplicationPdf;
using MTK.Modules.Hr.Application.VacationApplications.GetVacationApplicationById;
using MTK.Modules.Hr.Application.VacationApplications.SearchVacationApplications;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MTK.Modules.Hr.Presentation.Controllers;

[RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
public class VacationApplicationsController : BaseController
{
    public VacationApplicationsController(ISender sender) : base(sender)
    {
    }

    /// <summary>
    /// Yeni ödənişli məzuniyyət ərizəsi yaradır
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateVacationApplicationCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// ID-yə görə ödənişli məzuniyyət ərizəsini götür
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetVacationApplicationByIdQuery(id), cancellationToken);
        return result.IsSuccess ? Success(result.Value) : NotFound(result.Error);
    }

    /// <summary>
    /// Ödənişli məzuniyyət ərizələrini axtar (pagination ilə)
    /// </summary>
    [HttpPost("search")]
    public async Task<IActionResult> Search(
        [FromBody] SearchVacationApplicationsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Ödənişli məzuniyyət ərizəsini sil (yalnız PendingApproval statusunda)
    /// </summary>
    /// <remarks>
    /// Qeyd: Update funksionallığı yoxdur. Dəyişiklik etmək üçün DELETE + CREATE pattern istifadə edin.
    /// </remarks>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteVacationApplicationCommand(id), cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    /// <summary>
    /// Ödənişli məzuniyyət ərizəsini PDF formatında export et
    /// </summary>
    [HttpGet("{id:guid}/export-pdf")]
    public async Task<IActionResult> ExportPdf(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ExportVacationApplicationPdfQuery(id), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        using var memoryStream = new MemoryStream();
        await result.Value.FileStream.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        return File(
            memoryStream.ToArray(),
            "application/pdf",
            result.Value.FileName);
    }

    /// <summary>
    /// VacationApplication-ı VacationOrder-ə çevirir
    /// </summary>
    [HttpPost("{id:guid}/convert-to-order")]
    public async Task<IActionResult> ConvertToOrder(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new ConvertVacationToOrderCommand(id),
            cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }
}
