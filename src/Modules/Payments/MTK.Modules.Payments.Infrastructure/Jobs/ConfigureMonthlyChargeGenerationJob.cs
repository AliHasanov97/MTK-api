using Microsoft.Extensions.Options;
using Quartz;

namespace MTK.Modules.Payments.Infrastructure.Jobs;

internal sealed class ConfigureMonthlyChargeGenerationJob(
    IOptions<MonthlyChargeGenerationOptions> options)
    : IConfigureOptions<QuartzOptions>
{
    private readonly MonthlyChargeGenerationOptions _options = options.Value;

    public void Configure(QuartzOptions quartzOptions)
    {
        string jobName = typeof(MonthlyChargeGenerationJob).FullName!;

        quartzOptions
            .AddJob<MonthlyChargeGenerationJob>(configure => configure.WithIdentity(jobName))
            .AddTrigger(configure =>
                configure
                    .ForJob(jobName)
                    .WithCronSchedule(_options.CronExpression)); // Her ayın 1-i saat 00:00-da işləyir
    }
}
