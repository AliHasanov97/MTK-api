using MTK.Common.Application.Authorization;

using MTK.Modules.Hr.Application.EmploymentStatusChangeApplications.ConvertToOrder;
using MTK.Modules.Hr.Application.EmploymentStatusChangeApplications.CreateEmploymentStatusChangeApplication;
using MTK.Modules.Hr.Application.EmploymentStatusChangeApplications.DeleteEmploymentStatusChangeApplication;
using MTK.Modules.Hr.Application.EmploymentStatusChangeApplications.ExportEmploymentStatusChangeApplicationPdf;
using MTK.Modules.Hr.Application.EmploymentStatusChangeApplications.GetEmploymentStatusChangeApplicationById;
using MTK.Modules.Hr.Application.EmploymentStatusChangeApplications.SearchEmploymentStatusChangeApplications;
using MTK.Modules.Hr.Application.EmploymentStatusChangeApplications.UpdateEmploymentStatusChangeApplication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MTK.Modules.Hr.Presentation.Controllers;

[RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
public class EmploymentStatusChangeApplicationsController : BaseController
{
    public EmploymentStatusChangeApplicationsController(ISender sender) : base(sender)
    {
    }

    /// <summary>
    /// İş rejimi dəyişikliyi ərizəsi yaradır
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateEmploymentStatusChangeApplicationCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// İş rejimi dəyişikliyi ərizəsini yeniləyir
    /// </summary>
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateEmploymentStatusChangeApplicationCommand command,
        CancellationToken cancellationToken)
    {
        command.Id = id;
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    /// <summary>
    /// İş rejimi dəyişikliyi ərizəsini silir
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteEmploymentStatusChangeApplicationCommand(id), cancellationToken);
        return result.IsSuccess ? Success() : BadRequest(result.Error);
    }

    /// <summary>
    /// İş rejimi dəyişikliyi ərizəsini Id ilə gətirir
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetEmploymentStatusChangeApplicationByIdQuery(id), cancellationToken);
        return result.IsSuccess ? Success(result.Value) : NotFound(result.Error);
    }

    /// <summary>
    /// İş rejimi dəyişikliyi ərizələrini axtarır
    /// </summary>
    [HttpPost("search")]
    public async Task<IActionResult> Search(
        [FromBody] SearchEmploymentStatusChangeApplicationsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// İş rejimi dəyişikliyi ərizəsini əmrə çevirir
    /// </summary>
    [HttpPost("{id:guid}/convert-to-order")]
    public async Task<IActionResult> ConvertToOrder(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new ConvertEmploymentStatusChangeApplicationToOrderCommand { ApplicationId = id },
            cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// İş rejimi dəyişikliyi ərizəsini PDF formatında export edir
    /// </summary>
    [HttpGet("{id:guid}/export-pdf")]
    public async Task<IActionResult> ExportPdf(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new ExportEmploymentStatusChangeApplicationPdfQuery(id),
            cancellationToken);

        if (result.IsSuccess)
        {
            var response = result.Value;
            return File(response.FileStream, "application/pdf", response.FileName);
        }

        return NotFound(result.Error);
    }
}
