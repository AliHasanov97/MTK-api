using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MTK.Common.Application.Data;
using MTK.Common.Application.Messaging;
using MTK.Common.Infrastructure.Outbox;
using Quartz;
using System.Reflection;

namespace MTK.Modules.Warehouse.Infrastructure.Outbox;

[DisallowConcurrentExecution]
internal sealed class ProcessOutboxJob(
    IDbConnectionFactory dbConnectionFactory,
    IServiceScopeFactory serviceScopeFactory,
    IDateTimeProvider dateTimeProvider,
    IOptions<OutboxOptions> outboxOptions,
    ILogger<ProcessOutboxJob> logger)
    : ProcessOutboxJobBase(dbConnectionFactory, serviceScopeFactory, dateTimeProvider, outboxOptions, logger)
{
    protected override string ModuleName => "Warehouse";
    protected override string Schema => "warehouse";
    protected override Assembly HandlerAssembly => typeof(Application.Nomenclatures.Commands.CreateNomenclature.CreateNomenclatureCommand).Assembly;
}
