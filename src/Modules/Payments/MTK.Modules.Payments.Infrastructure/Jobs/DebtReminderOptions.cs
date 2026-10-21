namespace MTK.Modules.Payments.Infrastructure.Jobs;

internal sealed class DebtReminderOptions
{
    public string CronExpression { get; init; } = "0 0 9 * * ?";
    public string TimeZoneId { get; init; } = "Asia/Baku";
}
