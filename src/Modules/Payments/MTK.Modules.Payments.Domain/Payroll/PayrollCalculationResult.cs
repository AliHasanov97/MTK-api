namespace MTK.Modules.Payments.Domain.Payroll;

/// <summary>
/// <see cref="PayrollCalculator"/>-ın saf nəticəsi — bir işçi üçün bir ay üzrə
/// əmək haqqı hesablamasının bütün komponentləri. Entity deyil, sadəcə dəyər.
/// </summary>
public sealed record PayrollCalculationResult(
    decimal BaseSalary,
    decimal BonusAmount,
    // HR düzəlişinin pul ekvivalenti (saat əsaslı; işarəli: mənfi = maaşdan çıxılma).
    decimal AdjustmentAmount,
    decimal GrossSalary,
    decimal FinalNetSalary,
    decimal TotalEmployerCost);
