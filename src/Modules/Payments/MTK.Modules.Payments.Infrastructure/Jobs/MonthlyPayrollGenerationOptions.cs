namespace MTK.Modules.Payments.Infrastructure.Jobs;

internal sealed class MonthlyPayrollGenerationOptions
{
    // Quartz cron (saniyə dəqiqə saat ayın-günü ay həftənin-günü). Susmaya görə hər gün 01:00:
    // iş idempotentdir — əvvəlki ay üçün hələ PayrollRun yoxdursa yaradır. Beləliklə API-nin
    // ayın 1-ində dayandığı gün buraxılmır, növbəti işləmədə tutulur.
    public string CronExpression { get; init; } = "0 0 1 * * ?";

    // Cron-un və dövrün ("əvvəlki təqvim ayı") hesablandığı vaxt qurşağı.
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
}
