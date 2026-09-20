using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MTK.Common.Application.Data;
using MTK.Common.Application.Messaging;
using MTK.Common.Infrastructure.Inbox;
using Quartz;
using System.Reflection;

namespace MTK.Modules.Identity.Infrastructure.Inbox;

[DisallowConcurrentExecution]
internal sealed class ProcessInboxJob(
    IDbConnectionFactory dbConnectionFactory,
    IServiceScopeFactory serviceScopeFactory,
    IDateTimeProvider dateTimeProvider,
    IOptions<InboxOptions> inboxOptions,
    ILogger<ProcessInboxJob> logger)
    : ProcessInboxJobBase(dbConnectionFactory, serviceScopeFactory, dateTimeProvider, inboxOptions, logger)
{
    protected override string ModuleName => "Identity";
    protected override string Schema => "identity";
    protected override Assembly HandlerAssembly => Application.AssemblyReference.Assembly;
}
