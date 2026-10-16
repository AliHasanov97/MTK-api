namespace MTK.Modules.Payments.Domain.Payroll;

/// <summary>
/// Sadələşdirilmiş əmək haqqı hesablaması — vergi, sığorta və məhkəmə tutulmaları olmadan.
/// Yalnız əsas maaş, bonus və düzəliş hesablanır.
/// </summary>
public static class PayrollCalculator
{
    public static PayrollCalculationResult Calculate(
        decimal baseSalary,
        decimal bonusAmount,
        decimal adjustmentAmount)
    {
        if (baseSalary < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(baseSalary), "Əmək haqqı mənfi ola bilməz");
        }

        if (bonusAmount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bonusAmount), "Bonus mənfi ola bilməz");
        }

        // Bonus əmək haqqına əlavə olunur.
        // adjustmentAmount çağıran tərəfindən iş saatı əsasında hesablanır
        // (baza maaş × saat / saat norması) və işarəlidir — mənfi olduqda maaşdan çıxılır.
        // Gross heç vaxt 0-dan aşağı düşmür.
        var grossSalary = Math.Max(0m, baseSalary + bonusAmount + adjustmentAmount);

        // Heç bir tutma yoxdur - Final = Gross
        var finalNetSalary = grossSalary;

        // İşəgötürənin əlavə xərci yoxdur - Total Cost = Gross
        var totalEmployerCost = grossSalary;

        return new PayrollCalculationResult(
            BaseSalary: baseSalary,
            BonusAmount: bonusAmount,
            AdjustmentAmount: adjustmentAmount,
            GrossSalary: grossSalary,
            FinalNetSalary: finalNetSalary,
            TotalEmployerCost: totalEmployerCost);
    }
}
