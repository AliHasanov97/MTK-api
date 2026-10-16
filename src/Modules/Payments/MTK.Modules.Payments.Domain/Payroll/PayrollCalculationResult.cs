namespace MTK.Modules.Payments.Domain.Payroll;

/// <summary>
/// Sadələşdirilmiş maaş hesablama nəticəsi — heç bir tutma yoxdur.
/// </summary>
public sealed record PayrollCalculationResult(
    decimal BaseSalary,
    decimal BonusAmount,
    // HR düzəlişinin pul ekvivalenti (saat əsaslı; işarəli: mənfi = maaşdan çıxılma).
    decimal AdjustmentAmount,
    decimal GrossSalary,
    decimal FinalNetSalary,
    decimal TotalEmployerCost);
