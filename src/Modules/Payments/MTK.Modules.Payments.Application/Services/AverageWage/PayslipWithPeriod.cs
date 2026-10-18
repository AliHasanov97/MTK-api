namespace MTK.Modules.Payments.Application.Services.AverageWage;

/// <summary>
/// Orta əmək haqqı hesablaması üçün Payslip məlumatları (tarix ilə birlikdə).
/// </summary>
public sealed record PayslipWithPeriod(
    int Year,
    int Month,
    decimal BaseSalary,
    decimal BonusAmount,
    decimal AdjustmentAmount,
    decimal GrossSalary);
