using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace MTK.Modules.Payments.Infrastructure.Jobs;

internal sealed class ConfigureMonthlyChargeGenerationJob(
    IOptions<MonthlyChargeGenerationOptions> options,
    IHostEnvironment environment,
    ILogger<ConfigureMonthlyChargeGenerationJob> logger)
    : IConfigureOptions<QuartzOptions>
{
    private readonly MonthlyChargeGenerationOptions _options = options.Value;

    public void Configure(QuartzOptions quartzOptions)
    {
        string jobName = typeof(MonthlyChargeGenerationJob).FullName!;
        TimeZoneInfo timeZone = _options.ResolveTimeZone();

        // Yanlış cron ifadəsi səssizcə işləməməsin: başlanğıcda dərhal xəta
        if (!CronExpression.IsValidExpression(_options.CronExpression))
        {
            throw new InvalidOperationException(
                $"Payments:MonthlyChargeGeneration:CronExpression '{_options.CronExpression}' düzgün Quartz cron ifadəsi deyil.");
        }

        WarnIfTooFrequent(timeZone);

        quartzOptions
            .AddJob<MonthlyChargeGenerationJob>(configure => configure.WithIdentity(jobName))
            .AddTrigger(configure =>
                configure
                    .ForJob(jobName)
                    .WithCronSchedule(_options.CronExpression, cron => cron.InTimeZone(timeZone)));
    }

    /// <summary>Development-dən başqa mühitdə saatdan tez işləyən cron (məs. hər dəqiqə) ehtimal ki, test qalığıdır.</summary>
    private void WarnIfTooFrequent(TimeZoneInfo timeZone)
    {
        if (environment.IsDevelopment())
            return;

        var cron = new CronExpression(_options.CronExpression) { TimeZone = timeZone };
        DateTimeOffset? first = cron.GetNextValidTimeAfter(DateTimeOffset.UtcNow);
        DateTimeOffset? second = first is null ? null : cron.GetNextValidTimeAfter(first.Value);

        if (first is not null && second is not null && second.Value - first.Value < TimeSpan.FromHours(1))
        {
            logger.LogWarning(
                "Payments - MonthlyChargeGeneration cron '{Cron}' saatdan tez işləyir. Bu, test üçün nəzərdə tutulub; canlı mühitdə günlük cron tövsiyə olunur.",
                _options.CronExpression);
        }
    }
}
