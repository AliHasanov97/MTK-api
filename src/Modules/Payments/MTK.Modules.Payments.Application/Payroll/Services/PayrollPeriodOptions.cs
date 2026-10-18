namespace MTK.Modules.Payments.Application.Payroll.Services;

/// <summary>
/// Əmək haqqı hesablamasının dövr qaydası: ay bitməmiş hesablanmır — yalnız **keçmiş təqvim ayı**
/// üçün yeni hesablama yaradıla bilər (məs. noyabrda yalnız oktyabr). Vaxt qurşağı avtomatik
/// Quartz işi ilə eyni açardan oxunur (Payments:MonthlyPayrollGeneration:TimeZoneId).
/// </summary>
public sealed class PayrollPeriodOptions
{
    public string TimeZoneId { get; init; } = "Asia/Baku";

    public TimeZoneInfo ResolveTimeZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
        }
        catch (TimeZoneNotFoundException ex)
        {
            throw new InvalidOperationException(
                $"Payments:MonthlyPayrollGeneration:TimeZoneId '{TimeZoneId}' tapılmadı (məs. 'Asia/Baku').", ex);
        }
    }

    /// <summary>Hazırda hesablanması icazəli olan dövr — verilmiş UTC vaxtına görə keçmiş təqvim ayı.</summary>
    public (int Year, int Month) ResolveAllowedPeriod(DateTime utcNow)
    {
        var localNow = TimeZoneInfo.ConvertTime(new DateTimeOffset(utcNow, TimeSpan.Zero), ResolveTimeZone());
        var previousMonth = new DateOnly(localNow.Year, localNow.Month, 1).AddMonths(-1);
        return (previousMonth.Year, previousMonth.Month);
    }
}
