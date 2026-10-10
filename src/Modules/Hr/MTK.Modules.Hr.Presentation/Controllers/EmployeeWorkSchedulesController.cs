using MTK.Common.Application.Authorization;

using MTK.Modules.Hr.Application.EmployeeWorkSchedules.GetEmployeeWorkSchedule;
using MTK.Modules.Hr.Application.EmployeeWorkSchedules.SetEmployeeWorkSchedule;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MTK.Modules.Hr.Presentation.Controllers;

[RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
public class EmployeeWorkSchedulesController : BaseController
{
    public EmployeeWorkSchedulesController(ISender sender) : base(sender)
    {
    }

    /// <summary>
    /// İşçinin həftəlik iş qrafikini təyin edir
    /// </summary>
    [HttpPatch("employee/{employeeId:guid}")]
    public async Task<IActionResult> SetSchedule(
        [FromRoute] Guid employeeId,
        [FromBody] SetEmployeeWorkScheduleCommand command,
        CancellationToken cancellationToken)
    {
        command.EmployeeId = employeeId;
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// İşçinin həftəlik iş qrafikini qaytarır
    /// </summary>
    [HttpGet("employee/{employeeId:guid}")]
    public async Task<IActionResult> GetSchedule(
        [FromRoute] Guid employeeId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetEmployeeWorkScheduleQuery { EmployeeId = employeeId }, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : NotFound(result.Error);
    }
}
