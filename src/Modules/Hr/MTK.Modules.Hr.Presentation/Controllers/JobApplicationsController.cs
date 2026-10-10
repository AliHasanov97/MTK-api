using MTK.Common.Application.Authorization;

using MTK.Modules.Hr.Application.JobApplications.AddJobApplication;
using MTK.Modules.Hr.Application.JobApplications.ConvertToEmploymentOrder;
using MTK.Modules.Hr.Application.JobApplications.DeleteJobApplication;
using MTK.Modules.Hr.Application.JobApplications.ExportJobApplicationPdf;
using MTK.Modules.Hr.Application.JobApplications.GetJobApplicationById;
using MTK.Modules.Hr.Application.JobApplications.SearchJobApplications;
using MTK.Modules.Hr.Application.JobApplications.UpdateJobApplication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MTK.Modules.Hr.Presentation.Controllers;

[RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
public class JobApplicationsController : BaseController
{
    public JobApplicationsController(ISender sender) : base(sender)
    {
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddJobApplicationCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateJobApplicationCommand command, CancellationToken cancellationToken)
    {
        command.Id = id;
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteJobApplicationCommand(id), cancellationToken);
        return result.IsSuccess ? Success() : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetJobApplicationByIdQuery(id), cancellationToken);
        return result.IsSuccess ? Success(result.Value) : NotFound(result.Error);
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] SearchJobApplicationsQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}/export-pdf")]
    public async Task<IActionResult> ExportPdf([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ExportJobApplicationPdfQuery(id), cancellationToken);
        if (result.IsSuccess)
        {
            var response = result.Value;
            return File(response.FileStream, "application/pdf", response.FileName);
        }

        return NotFound(result.Error);
    }

    [HttpPost("{id:guid}/convert-to-employment-order")]
    public async Task<IActionResult> ConvertToEmploymentOrder([FromRoute] Guid id, [FromBody] ConvertJobApplicationToEmploymentOrderCommand command, CancellationToken cancellationToken)
    {
        command.JobApplicationId = id;
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }
}
