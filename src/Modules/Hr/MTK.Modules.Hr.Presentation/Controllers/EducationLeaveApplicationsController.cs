using MTK.Common.Application.Authorization;
using MTK.Common.Presentation.Responses;

using MTK.Modules.Hr.Application.EducationLeaveApplications.ConvertToOrder;
using MTK.Modules.Hr.Application.EducationLeaveApplications.CreateEducationLeaveApplication;
using MTK.Modules.Hr.Application.EducationLeaveApplications.DeleteEducationLeaveApplication;
using MTK.Modules.Hr.Application.EducationLeaveApplications.ExportEducationLeaveApplicationPdf;
using MTK.Modules.Hr.Application.EducationLeaveApplications.GetEducationLeaveApplicationById;
using MTK.Modules.Hr.Application.EducationLeaveApplications.SearchEducationLeaveApplications;
using MTK.Modules.Hr.Application.EducationLeaveApplications.UpdateEducationLeaveApplication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MTK.Modules.Hr.Presentation.Controllers;

[RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
public class EducationLeaveApplicationsController : BaseController
{
    public EducationLeaveApplicationsController(ISender sender) : base(sender)
    {
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateEducationLeaveApplicationCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateEducationLeaveApplicationCommand command,
        CancellationToken cancellationToken)
    {
        command.Id = id;
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Success() : BadRequest(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteEducationLeaveApplicationCommand(id), cancellationToken);
        return result.IsSuccess ? Success() : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetEducationLeaveApplicationByIdQuery(id), cancellationToken);
        return result.IsSuccess ? Success(result.Value) : NotFound(result.Error);
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search(
        [FromBody] SearchEducationLeaveApplicationsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// EducationLeaveApplication-ı EducationLeaveOrder-ə çevirir
    /// </summary>
    [HttpPost("{id:guid}/convert-to-order")]
    public async Task<IActionResult> ConvertToOrder(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new ConvertEducationLeaveToOrderCommand(id),
            cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Təhsil məzuniyyəti ərizəsini PDF formatında export edir
    /// </summary>
    [HttpGet("{id:guid}/export-pdf")]
    public async Task<IActionResult> ExportPdf(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ExportEducationLeaveApplicationPdfQuery(id), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return File(result.Value.FileStream, "application/pdf", result.Value.FileName);
    }
}
