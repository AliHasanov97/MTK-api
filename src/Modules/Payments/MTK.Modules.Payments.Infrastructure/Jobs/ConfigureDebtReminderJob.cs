using Microsoft.Extensions.Options;
using Quartz;

namespace MTK.Modules.Payments.Infrastructure.Jobs;

internal sealed class ConfigureDebtReminderJob(IOptions<DebtReminderOptions> options)
    : IConfigureOptions<QuartzOptions>
{
    public void Configure(QuartzOptions quartzOptions)
    {
        var cron = options.Value.CronExpression;
        if (!CronExpression.IsValidExpression(cron))
            throw new InvalidOperationException($"Payments:DebtReminders:CronExpression '{cron}' is not a valid Quartz cron expression.");

        TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(options.Value.TimeZoneId);
        string jobName = typeof(DebtReminderJob).FullName!;
        quartzOptions
            .AddJob<DebtReminderJob>(job => job.WithIdentity(jobName))
            .AddTrigger(trigger => trigger
                .ForJob(jobName)
                .WithCronSchedule(cron, schedule => schedule.InTimeZone(timeZone)));
    }
}
