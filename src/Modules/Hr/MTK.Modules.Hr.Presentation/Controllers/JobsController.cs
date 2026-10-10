using MTK.Common.Application.Authorization;
using MTK.Modules.Hr.Application.Jobs.AddJob;
using MTK.Modules.Hr.Application.Jobs.DeleteJob;
using MTK.Modules.Hr.Application.Jobs.GetJobById;
using MTK.Modules.Hr.Application.Jobs.SearchJobs;
using MTK.Modules.Hr.Application.Jobs.UpdateJob;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MTK.Modules.Hr.Presentation.Controllers;

[RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
public class JobsController : BaseController
{
    public JobsController(ISender sender) : base(sender)
    {
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddJobCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateJobCommand command, CancellationToken cancellationToken)
    {
        command.JobId = id;
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteJobCommand(id), cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetJobByIdQuery(id), cancellationToken);
        return result.IsSuccess ? Success(result.Value) : NotFound(result.Error);
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] SearchJobsQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }
}
