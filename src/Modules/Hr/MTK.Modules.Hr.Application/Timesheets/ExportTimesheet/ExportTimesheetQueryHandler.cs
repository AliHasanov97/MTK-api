using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Organization;
using MTK.Modules.Hr.Application.Timesheets.GetTimesheets;

using MediatR;

namespace MTK.Modules.Hr.Application.Timesheets.ExportTimesheet;

internal sealed class ExportTimesheetQueryHandler : IQueryHandler<ExportTimesheetQuery, ExportTimesheetResponse>
{
    private readonly ISender _sender;
    private readonly ITimesheetExportService _exportService;
    private readonly IOrganizationInfo _organization;

    public ExportTimesheetQueryHandler(
        ISender sender,
        ITimesheetExportService exportService,
        IOrganizationInfo organization)
    {
        _sender = sender;
        _exportService = exportService;
        _organization = organization;
    }

    public async Task<Result<ExportTimesheetResponse>> Handle(
        ExportTimesheetQuery request,
        CancellationToken cancellationToken)
    {
        // Timesheet məlumatlarını al
        var timesheetQuery = new GetTimesheetsQuery
        {
            Year = request.Year,
            Month = request.Month
        };

        var timesheetResult = await _sender.Send(timesheetQuery, cancellationToken);
        if (timesheetResult.IsFailure)
            return Result.Failure<ExportTimesheetResponse>(timesheetResult.Error);

        // Excel-ə export et
        var (stream, fileName) = await _exportService.ExportToExcelAsync(
            timesheetResult.Value,
            _organization.Name,
            _organization.Director,
            cancellationToken);

        return Result.Success(new ExportTimesheetResponse
        {
            FileContent = stream.ToArray(),
            FileName = fileName,
            ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        });
    }
}
