using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Modules.Reports.Application.MonthlyReports;

namespace MTK.Modules.Reports.Presentation.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize]
[RequireAnyRole(Roles.Admin, Roles.Accountant, Roles.BuildingManager)]
public sealed class ReportsController : ControllerBase
{
    private readonly ISender _sender;

    public ReportsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Seçilmiş ay üzrə tam aylıq hesabat (maliyyə + anbar).</summary>
    [HttpGet("monthly")]
    public async Task<IActionResult> GetMonthlyReport(
        [FromQuery] int year,
        [FromQuery] int month,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetMonthlyReportQuery(year, month), cancellationToken);

        return result.IsSuccess
            ? Ok(new { data = result.Value, message = "Uğurla icra edildi" })
            : BadRequest(new { error = result.Error.Message, code = result.Error.Code });
    }

    /// <summary>Seçilmiş ay üzrə aylıq hesabatın PDF faylı.</summary>
    [HttpGet("monthly/export-pdf")]
    public async Task<IActionResult> ExportMonthlyReportPdf(
        [FromQuery] int year,
        [FromQuery] int month,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ExportMonthlyReportPdfQuery(year, month), cancellationToken);

        return result.IsSuccess
            ? File(result.Value.FileContent, result.Value.ContentType, result.Value.FileName)
            : BadRequest(new { error = result.Error.Message, code = result.Error.Code });
    }
}
