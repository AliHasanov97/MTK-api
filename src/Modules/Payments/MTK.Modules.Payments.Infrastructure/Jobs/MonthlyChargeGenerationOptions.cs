namespace MTK.Modules.Payments.Infrastructure.Jobs;

internal sealed class MonthlyChargeGenerationOptions
{
    // Cron expression: "0 0 1 * * ?"
    // Mənası: Hər ayın 1-i, saat 00:00-da (gecə yarısı) işləyir
    // 0 = 0 saniyə
    // 0 = 0 dəqiqə
    // 1 = Saat 1 (gecə yarısından sonra 1-ci saat, yəni 00:00)
    // * = Hər ayın
    // * = 1-ci günü
    // ? = İstənilən həftənin günü
    public string CronExpression { get; init; } = "0 0 1 * * ?";
}
