using MTK.Common.Application.Authorization;
using MTK.Common.Presentation.Responses;

using MTK.Modules.Hr.Application.VacationCompensationApplications.ConvertToOrder;
using MTK.Modules.Hr.Application.VacationCompensationApplications.CreateVacationCompensationApplication;
using MTK.Modules.Hr.Application.VacationCompensationApplications.DeleteVacationCompensationApplication;
using MTK.Modules.Hr.Application.VacationCompensationApplications.ExportVacationCompensationApplicationPdf;
using MTK.Modules.Hr.Application.VacationCompensationApplications.GetVacationCompensationApplicationById;
using MTK.Modules.Hr.Application.VacationCompensationApplications.SearchVacationCompensationApplications;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MTK.Modules.Hr.Presentation.Controllers;

[RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
public class VacationCompensationApplicationsController : BaseController
{
    public VacationCompensationApplicationsController(ISender sender) : base(sender)
    {
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateVacationCompensationApplicationCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteVacationCompensationApplicationCommand(id), cancellationToken);
        return result.IsSuccess ? Success() : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetVacationCompensationApplicationByIdQuery(id), cancellationToken);
        return result.IsSuccess ? Success(result.Value) : NotFound(result.Error);
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search(
        [FromBody] SearchVacationCompensationApplicationsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// VacationCompensationApplication-ı CompensationOrder-ə çevirir
    /// </summary>
    [HttpPost("{id:guid}/convert-to-order")]
    public async Task<IActionResult> ConvertToOrder(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new ConvertVacationCompensationToOrderCommand(id),
            cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// VacationCompensationApplication-ı PDF-ə export edir
    /// </summary>
    [HttpGet("{id:guid}/export-pdf")]
    public async Task<IActionResult> ExportPdf(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new ExportVacationCompensationApplicationPdfQuery(id),
            cancellationToken);

        if (!result.IsSuccess)
            return NotFound(result.Error);

        return File(result.Value.FileStream, "application/pdf", result.Value.FileName);
    }
}