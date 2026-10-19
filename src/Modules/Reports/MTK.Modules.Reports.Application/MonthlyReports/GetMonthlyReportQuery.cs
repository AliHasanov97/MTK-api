using MTK.Common.Application.Messaging;

namespace MTK.Modules.Reports.Application.MonthlyReports;

public sealed record GetMonthlyReportQuery(int Year, int Month)
    : IQuery<MonthlyReportResponse>;
