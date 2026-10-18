namespace MTK.Modules.Payments.Application.Services.AverageWage;

/// <summary>
/// Son 12 ayın orta əmək haqqı hesablamasının nəticəsi.
/// Azərbaycan Əmək Məcəlləsinə (Maddə 145-146) uyğun hesablanır.
/// </summary>
public sealed record AverageWageResult(
    decimal AverageMonthlyWage,
    decimal AverageDailyWage,
    int MonthsIncluded,
    DateTime FromDate,
    DateTime ToDate,
    List<MonthlyWageBreakdown> Breakdown);

/// <summary>
/// Bir ayın maaş təfərrüatı.
/// </summary>
public sealed record MonthlyWageBreakdown(
    int Year,
    int Month,
    decimal BaseSalary,
    decimal TotalBonuses,
    decimal TotalAdjustments,
    decimal GrossWage);
