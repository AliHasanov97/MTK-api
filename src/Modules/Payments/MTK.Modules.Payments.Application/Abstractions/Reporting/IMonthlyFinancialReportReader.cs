using MTK.Modules.Payments.Application.Reports.GetMonthlyFinancialReport;

namespace MTK.Modules.Payments.Application.Abstractions.Reporting;

/// <summary>
/// Aylıq hesabat üçün oxu modeli. Yazma tərəfi deyil — heç bir entity izlənmir
/// və heç nə dəyişdirilmir; buna görə ayrıca repository aqreqatı kimi deyil,
/// birbaşa EF sorğuları ilə qurulur.
/// </summary>
public interface IMonthlyFinancialReportReader
{
    Task<MonthlyFinancialReportResponse> ReadAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default);
}
