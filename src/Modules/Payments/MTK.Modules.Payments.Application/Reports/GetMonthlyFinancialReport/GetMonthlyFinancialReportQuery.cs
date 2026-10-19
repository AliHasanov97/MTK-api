using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Reports.GetMonthlyFinancialReport;

public sealed record GetMonthlyFinancialReportQuery(int Year, int Month)
    : IQuery<MonthlyFinancialReportResponse>;
