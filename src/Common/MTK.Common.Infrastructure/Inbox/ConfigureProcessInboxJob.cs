using Microsoft.Extensions.Options;
using Quartz;

namespace MTK.Common.Infrastructure.Inbox;

public sealed class ConfigureProcessInboxJob<TJob>(IOptions<InboxOptions> inboxOptions)
    : IConfigureOptions<QuartzOptions>
    where TJob : ProcessInboxJobBase
{
    private readonly InboxOptions _inboxOptions = inboxOptions.Value;

    public void Configure(QuartzOptions options)
    {
        string jobName = typeof(TJob).FullName!;
        options
            .AddJob<TJob>(configure => configure.WithIdentity(jobName))
            .AddTrigger(configure =>
                configure
                    .ForJob(jobName)
                    .WithSimpleSchedule(schedule =>
                        schedule.WithIntervalInSeconds(_inboxOptions.IntervalInSeconds)
                                .RepeatForever()));
    }
}
