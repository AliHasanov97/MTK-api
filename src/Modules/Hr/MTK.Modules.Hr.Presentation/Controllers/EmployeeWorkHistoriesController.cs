using MTK.Common.Application.Authorization;

using MTK.Modules.Hr.Application.EmployeeWorkHistories.AddEmployeeWorkHistory;
using MTK.Modules.Hr.Application.EmployeeWorkHistories.DeleteEmployeeWorkHistory;
using MTK.Modules.Hr.Application.EmployeeWorkHistories.GetEmployeeWorkHistories;
using MTK.Modules.Hr.Application.EmployeeWorkHistories.UpdateEmployeeWorkHistory;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MTK.Modules.Hr.Presentation.Controllers;

[RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
public class EmployeeWorkHistoriesController : BaseController
{
    public EmployeeWorkHistoriesController(ISender sender) : base(sender)
    {
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddEmployeeWorkHistoryCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateEmployeeWorkHistoryCommand command, CancellationToken cancellationToken)
    {
        command.Id = id;
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteEmployeeWorkHistoryCommand(id), cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpGet("employee/{employeeId:guid}")]
    public async Task<IActionResult> GetByEmployeeId([FromRoute] Guid employeeId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetEmployeeWorkHistoriesQuery(employeeId), cancellationToken);
        return result.IsSuccess ? Success(result.Value) : NotFound(result.Error);
    }
}
