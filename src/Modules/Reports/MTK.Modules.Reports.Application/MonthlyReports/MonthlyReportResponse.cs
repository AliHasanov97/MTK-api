using MTK.Modules.Payments.Application.Reports.GetMonthlyFinancialReport;
using MTK.Modules.Warehouse.Application.Reports.GetWarehouseMonthlyReport;

namespace MTK.Modules.Reports.Application.MonthlyReports;

/// <summary>
/// Bir aylıq hesabat sənədi: Payments-in maliyyə hissəsi + Warehouse-un anbar hissəsi.
/// Bu tip yalnız oxuma/çap üçündür — heç bir modulun aqreqatını dəyişmir.
/// </summary>
public sealed record MonthlyReportResponse(
    MonthlyFinancialReportResponse Finance,
    WarehouseMonthlyReportResponse Warehouse,
    DateTimeOffset GeneratedAt);
