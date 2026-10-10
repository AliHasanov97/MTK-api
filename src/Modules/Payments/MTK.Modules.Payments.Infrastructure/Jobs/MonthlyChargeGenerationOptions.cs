namespace MTK.Modules.Payments.Infrastructure.Jobs;

internal sealed class MonthlyChargeGenerationOptions
{
    // Quartz cron (saniyə dəqiqə saat ayın-günü ay həftənin-günü). Susmaya görə hər gün 00:30:
    // iş idempotentdir — cari dövr üçün hələ borcu olmayan mənzil/qarajlara borc yaradır. Beləliklə
    // API-nin dayandığı gün, sonradan təyin olunan sahib və ya əlavə olunan tarif növbəti işləmədə tutulur.
    public string CronExpression { get; init; } = "0 30 0 * * ?";

    // Cron-un və dövrün ("yyyy-MM") hesablandığı vaxt qurşağı. Server harada işləyirsə işləsin
    // ayın sərhədi eyni olsun deyə açıq göstərilir.
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
                $"Payments:MonthlyChargeGeneration:TimeZoneId '{TimeZoneId}' tapılmadı (məs. 'Asia/Baku').", ex);
        }
    }
}
