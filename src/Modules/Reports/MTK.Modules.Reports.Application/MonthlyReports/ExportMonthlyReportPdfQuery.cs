using MTK.Common.Application.Exporting;
using MTK.Common.Application.Messaging;

namespace MTK.Modules.Reports.Application.MonthlyReports;

public sealed record ExportMonthlyReportPdfQuery(int Year, int Month)
    : IQuery<ExportFileResult>;
