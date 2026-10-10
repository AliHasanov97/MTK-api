using MTK.Modules.Hr.Application.Timesheets.GetTimesheets;

namespace MTK.Modules.Hr.Application.Timesheets.ExportTimesheet;

public interface ITimesheetExportService
{
    Task<(MemoryStream Stream, string FileName)> ExportToExcelAsync(
        GetTimesheetsResponse timesheetData,
        string companyName,
        string? directorName,
        CancellationToken cancellationToken = default);
}
