using MTK.Common.Application.Authorization;

using MTK.Modules.Hr.Application.EmployeeEducationHistories.AddEmployeeEducationHistory;
using MTK.Modules.Hr.Application.EmployeeEducationHistories.DeleteEmployeeEducationHistory;
using MTK.Modules.Hr.Application.EmployeeEducationHistories.GetEmployeeEducationHistories;
using MTK.Modules.Hr.Application.EmployeeEducationHistories.SearchEmployeeEducationHistories;
using MTK.Modules.Hr.Application.EmployeeEducationHistories.UpdateEmployeeEducationHistory;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MTK.Modules.Hr.Presentation.Controllers;

[RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
public class EmployeeEducationHistoriesController : BaseController
{
    public EmployeeEducationHistoriesController(ISender sender) : base(sender)
    {
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddEmployeeEducationHistoryCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateEmployeeEducationHistoryCommand command, CancellationToken cancellationToken)
    {
        command.Id = id;
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteEmployeeEducationHistoryCommand(id), cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpGet("employee/{employeeId:guid}")]
    public async Task<IActionResult> GetByEmployeeId([FromRoute] Guid employeeId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetEmployeeEducationHistoriesQuery(employeeId), cancellationToken);
        return result.IsSuccess ? Success(result.Value) : NotFound(result.Error);
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] SearchEmployeeEducationHistoriesQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }
}
