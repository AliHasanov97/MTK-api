using MTK.Common.Application.Authorization;

using MTK.Modules.Hr.Application.Timesheets.ExportTimesheet;
using MTK.Modules.Hr.Application.Timesheets.GetTimesheets;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MTK.Modules.Hr.Presentation.Controllers;

[RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
public class TimesheetsController : BaseController
{
    public TimesheetsController(ISender sender) : base(sender)
    {
    }

    /// <summary>
    /// Şirkətin aylıq timesheet-ini hesablayıb qaytarır
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetTimesheets(
        [FromQuery] int year,
        [FromQuery] int month,
        CancellationToken cancellationToken)
    {
        var query = new GetTimesheetsQuery
        {
            Year = year,
            Month = month
        };
        var result = await _sender.Send(query, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Aylıq timesheet-i Excel formatında export edir
    /// </summary>
    [HttpGet("export")]
    public async Task<IActionResult> ExportTimesheets(
        [FromQuery] int year,
        [FromQuery] int month,
        CancellationToken cancellationToken)
    {
        var query = new ExportTimesheetQuery
        {
            Year = year,
            Month = month
        };
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return File(
            result.Value.FileContent,
            result.Value.ContentType,
            result.Value.FileName);
    }
}
