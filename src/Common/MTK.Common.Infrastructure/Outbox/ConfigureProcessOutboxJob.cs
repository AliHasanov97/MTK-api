using Microsoft.Extensions.Options;
using Quartz;

namespace MTK.Common.Infrastructure.Outbox;

public sealed class ConfigureProcessOutboxJob<TJob>(IOptions<OutboxOptions> outboxOptions)
    : IConfigureOptions<QuartzOptions>
    where TJob : ProcessOutboxJobBase
{
    private readonly OutboxOptions _outboxOptions = outboxOptions.Value;

    public void Configure(QuartzOptions options)
    {
        string jobName = typeof(TJob).FullName!;
        options
            .AddJob<TJob>(configure => configure.WithIdentity(jobName))
            .AddTrigger(configure =>
                configure
                    .ForJob(jobName)
                    .WithSimpleSchedule(schedule =>
                        schedule.WithIntervalInSeconds(_outboxOptions.IntervalInSeconds)
                                .RepeatForever()));
    }
}
