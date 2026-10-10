using MTK.Common.Application.Authorization;

using MTK.Modules.Hr.Application.CalendarDays;
using MTK.Modules.Hr.Application.CalendarDays.BulkCreateCalendarDays;
using MTK.Modules.Hr.Application.CalendarDays.CreateCalendarDay;
using MTK.Modules.Hr.Application.CalendarDays.DeleteCalendarDay;
using MTK.Modules.Hr.Application.CalendarDays.GetCalendarDaysByYear;
using MTK.Modules.Hr.Application.CalendarDays.UpdateCalendarDay;
using MTK.Modules.Hr.Domain.Employees;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MTK.Modules.Hr.Presentation.Controllers;

[RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
public class CalendarDaysController : BaseController
{
    public CalendarDaysController(ISender sender) : base(sender)
    {
    }

    /// <summary>
    /// İl üzrə təqvim günlərini və aylıq statistikanı əldə edir.
    /// </summary>
    /// <param name="year">İl</param>
    /// <param name="workingDays">İş qrafiki (1=FiveDays, 2=SixDays). Verilsə, həmin qrafikə uyğun hesablanır.</param>
    /// <param name="cancellationToken"></param>
    [HttpGet]
    public async Task<IActionResult> GetByYear(
        [FromQuery] int year,
        [FromQuery] WorkingDays? workingDays,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetCalendarDaysByYearQuery(year, workingDays), cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCalendarDayCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Təqvim günlərini toplu əlavə edir (mövcud tarixlər atlanır).
    /// </summary>
    [HttpPost("bulk")]
    public async Task<IActionResult> BulkCreate([FromBody] BulkCreateCalendarDaysCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateCalendarDayCommand command, CancellationToken cancellationToken)
    {
        var updateCommand = command with { Id = id };
        var result = await _sender.Send(updateCommand, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteCalendarDayCommand(id), cancellationToken);
        return result.IsSuccess ? Success() : BadRequest(result.Error);
    }
}
