using MTK.Modules.Reports.Application.MonthlyReports;

namespace MTK.Modules.Reports.Application.Abstractions;

public interface IMonthlyReportPdfExportService
{
    MemoryStream ExportToPdf(MonthlyReportResponse report);
}
