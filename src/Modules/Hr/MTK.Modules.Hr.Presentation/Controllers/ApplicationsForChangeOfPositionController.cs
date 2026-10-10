using MTK.Common.Application.Authorization;

using MTK.Modules.Hr.Application.ApplicationsForChangeOfPosition.AddApplicationForChangeOfPosition;
using MTK.Modules.Hr.Application.ApplicationsForChangeOfPosition.ConvertToOrderForChangeOfPosition;
using MTK.Modules.Hr.Application.ApplicationsForChangeOfPosition.DeleteApplicationForChangeOfPosition;
using MTK.Modules.Hr.Application.ApplicationsForChangeOfPosition.ExportApplicationForChangeOfPositionPdf;
using MTK.Modules.Hr.Application.ApplicationsForChangeOfPosition.GetApplicationForChangeOfPositionById;
using MTK.Modules.Hr.Application.ApplicationsForChangeOfPosition.SearchApplicationsForChangeOfPosition;
using MTK.Modules.Hr.Application.ApplicationsForChangeOfPosition.UpdateApplicationForChangeOfPosition;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MTK.Modules.Hr.Presentation.Controllers;

[RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
public class ApplicationsForChangeOfPositionController : BaseController
{
    public ApplicationsForChangeOfPositionController(ISender sender) : base(sender)
    {
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddApplicationForChangeOfPositionCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateApplicationForChangeOfPositionCommand command, CancellationToken cancellationToken)
    {
        command.Id = id;
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteApplicationForChangeOfPositionCommand { Id = id }, cancellationToken);
        return result.IsSuccess ? Success() : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetApplicationForChangeOfPositionByIdQuery { Id = id }, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : NotFound(result.Error);
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] SearchApplicationsForChangeOfPositionQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("{id:guid}/convert-to-order")]
    public async Task<IActionResult> ConvertToOrder([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ConvertApplicationForChangeOfPositionToOrderCommand { ApplicationId = id }, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}/export-pdf")]
    public async Task<IActionResult> ExportPdf([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new ExportApplicationForChangeOfPositionPdfQuery(id),
            cancellationToken);

        if (result.IsSuccess)
        {
            var response = result.Value;
            return File(response.FileStream, "application/pdf", response.FileName);
        }

        return NotFound(result.Error);
    }
}
