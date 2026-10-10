using MTK.Common.Application.Authorization;
using MTK.Common.Presentation.Responses;

using MTK.Modules.Hr.Application.UnpaidLeaveApplications.ConvertToOrder;
using MTK.Modules.Hr.Application.UnpaidLeaveApplications.CreateUnpaidLeaveApplication;
using MTK.Modules.Hr.Application.UnpaidLeaveApplications.DeleteUnpaidLeaveApplication;
using MTK.Modules.Hr.Application.UnpaidLeaveApplications.ExportUnpaidLeaveApplicationPdf;
using MTK.Modules.Hr.Application.UnpaidLeaveApplications.GetUnpaidLeaveApplicationById;
using MTK.Modules.Hr.Application.UnpaidLeaveApplications.SearchUnpaidLeaveApplications;
using MTK.Modules.Hr.Application.UnpaidLeaveApplications.UpdateUnpaidLeaveApplication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MTK.Modules.Hr.Presentation.Controllers;

[RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
public class UnpaidLeaveApplicationsController : BaseController
{
    public UnpaidLeaveApplicationsController(ISender sender) : base(sender)
    {
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateUnpaidLeaveApplicationCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateUnpaidLeaveApplicationCommand command,
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
        var result = await _sender.Send(new DeleteUnpaidLeaveApplicationCommand(id), cancellationToken);
        return result.IsSuccess ? Success() : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetUnpaidLeaveApplicationByIdQuery(id), cancellationToken);
        return result.IsSuccess ? Success(result.Value) : NotFound(result.Error);
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search(
        [FromBody] SearchUnpaidLeaveApplicationsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// UnpaidLeaveApplication-ı UnpaidLeaveOrder-ə çevirir
    /// </summary>
    [HttpPost("{id:guid}/convert-to-order")]
    public async Task<IActionResult> ConvertToOrder(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new ConvertUnpaidLeaveToOrderCommand(id),
            cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Ödənişsiz məzuniyyət ərizəsini PDF formatında export edir
    /// </summary>
    [HttpGet("{id:guid}/export-pdf")]
    public async Task<IActionResult> ExportPdf(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ExportUnpaidLeaveApplicationPdfQuery(id), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return File(result.Value.FileStream, "application/pdf", result.Value.FileName);
    }
}
