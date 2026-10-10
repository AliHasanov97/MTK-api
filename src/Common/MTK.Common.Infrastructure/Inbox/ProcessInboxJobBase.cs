using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MTK.Common.Application.Data;
using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Common.Infrastructure.Serialization;
using Newtonsoft.Json;
using Npgsql;
using Quartz;
using System.Data.Common;
using System.Reflection;

namespace MTK.Common.Infrastructure.Inbox;

[DisallowConcurrentExecution]
public abstract class ProcessInboxJobBase(
    IDbConnectionFactory dbConnectionFactory,
    IServiceScopeFactory serviceScopeFactory,
    IDateTimeProvider dateTimeProvider,
    IOptions<InboxOptions> inboxOptions,
    ILogger logger) : IJob
{
    protected abstract string ModuleName { get; }
    protected abstract string Schema { get; }
    protected abstract Assembly HandlerAssembly { get; }

    private readonly InboxOptions _inboxOptions = inboxOptions.Value;

    public async Task Execute(IJobExecutionContext context)
    {
        DbConnection connection;
        try
        {
            connection = await dbConnectionFactory.OpenConnectionAsync();
        }
        catch (PostgresException ex) when (ex.SqlState == "53300")
        {
            logger.LogWarning("{Module} - Connection exhaustion, will retry on next run", ModuleName);
            return;
        }

        await using var _ = connection;
        await using DbTransaction transaction = await connection.BeginTransactionAsync();

        try
        {
            IReadOnlyList<InboxMessageResponse> inboxMessages =
                await GetInboxMessagesAsync(connection, transaction);

            if (inboxMessages.Count == 0)
            {
                // Boş işləmə: heç nə loglanmır (hər 5 saniyədə səs-küy yaratmasın)
                await transaction.CommitAsync(context.CancellationToken);
                return;
            }

            logger.LogInformation("{Module} - Processing {Count} inbox message(s)", ModuleName, inboxMessages.Count);

            foreach (InboxMessageResponse inboxMessage in inboxMessages)
            {
                Exception? exception = null;

                try
                {
                    // Tam məzmun yalnız Debug səviyyəsində (şəxsi məlumat ola bilər)
                    logger.LogDebug(
                        "{Module} - Inbox message {MessageId} content: {Content}",
                        ModuleName,
                        inboxMessage.Id,
                        inboxMessage.Content);

                    // Use SerializerSettings.Instance with MetadataPropertyHandling.ReadAhead
                    // This tells Newtonsoft.Json to read $type FIRST before deserializing
                    IIntegrationEvent integrationEvent = JsonConvert.DeserializeObject<IIntegrationEvent>(
                        inboxMessage.Content,
                        SerializerSettings.Instance)!;

                    using IServiceScope scope = serviceScopeFactory.CreateScope();

                    IEnumerable<IIntegrationEventHandler> handlers = GetHandlers(
                        integrationEvent.GetType(),
                        scope.ServiceProvider,
                        HandlerAssembly);

                    foreach (IIntegrationEventHandler integrationEventHandler in handlers)
                    {
                        string consumerName = integrationEventHandler.GetType().FullName!;

                        // Eyni handler eyni mesajı təkrar icra etməsin (inbox_message_consumers)
                        if (await ConsumerAlreadyHandledAsync(connection, transaction, inboxMessage.Id, consumerName))
                        {
                            logger.LogInformation(
                                "{Module} - Handler {HandlerType} already handled message {MessageId}, skipping",
                                ModuleName,
                                integrationEventHandler.GetType().Name,
                                inboxMessage.Id);
                            continue;
                        }

                        await integrationEventHandler.Handle(integrationEvent, context.CancellationToken);

                        // Uğurlu icra dərhal qeyd olunur
                        await InsertConsumerAsync(connection, transaction, inboxMessage.Id, consumerName);

                    }

                    logger.LogInformation(
                        "{Module} - Inbox message {MessageId} ({EventType}) processed",
                        ModuleName,
                        inboxMessage.Id,
                        integrationEvent.GetType().Name);
                }
                catch (Exception caughtException)
                {
                    logger.LogError(
                        caughtException,
                        "{Module} - Exception while processing inbox message {MessageId}",
                        ModuleName,
                        inboxMessage.Id);

                    exception = caughtException;
                }

                await UpdateInboxMessageAsync(connection, transaction, inboxMessage, exception);
            }

            await transaction.CommitAsync(context.CancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Module} - Error processing inbox messages, rolling back transaction", ModuleName);
            await transaction.RollbackAsync(context.CancellationToken);
            throw;
        }
    }

    private async Task<IReadOnlyList<InboxMessageResponse>> GetInboxMessagesAsync(
        DbConnection connection,
        DbTransaction transaction)
    {
        string sql =
            $"""
             SELECT
                "Id" AS {nameof(InboxMessageResponse.Id)},
                "Content" AS {nameof(InboxMessageResponse.Content)}
             FROM {Schema}.inbox_messages
             WHERE "ProcessedOnUtc" IS NULL
             ORDER BY "OccurredOnUtc"
             LIMIT {_inboxOptions.BatchSize}
             FOR UPDATE
             """;

        IEnumerable<InboxMessageResponse> inboxMessages = await connection.QueryAsync<InboxMessageResponse>(
            sql,
            transaction: transaction);

        return inboxMessages.AsList();
    }

    private async Task UpdateInboxMessageAsync(
        DbConnection connection,
        DbTransaction transaction,
        InboxMessageResponse inboxMessage,
        Exception? exception)
    {
        string sql =
            $"""
            UPDATE {Schema}.inbox_messages
            SET "ProcessedOnUtc" = @ProcessedOnUtc,
                "Error" = @Error
            WHERE "Id" = @Id
            """;

        await connection.ExecuteAsync(
            sql,
            new
            {
                inboxMessage.Id,
                ProcessedOnUtc = dateTimeProvider.UtcNow,
                Error = exception?.ToString()
            },
            transaction: transaction);
    }

    private async Task<bool> ConsumerAlreadyHandledAsync(
        DbConnection connection,
        DbTransaction transaction,
        Guid inboxMessageId,
        string consumerName)
    {
        return await connection.ExecuteScalarAsync<bool>(
            $"""
            SELECT EXISTS (
                SELECT 1 FROM {Schema}.inbox_message_consumers
                WHERE "InboxMessageId" = @InboxMessageId AND "Name" = @Name)
            """,
            new { InboxMessageId = inboxMessageId, Name = consumerName },
            transaction: transaction);
    }

    private async Task InsertConsumerAsync(
        DbConnection connection,
        DbTransaction transaction,
        Guid inboxMessageId,
        string consumerName)
    {
        await connection.ExecuteAsync(
            $"""
            INSERT INTO {Schema}.inbox_message_consumers ("InboxMessageId", "Name")
            VALUES (@InboxMessageId, @Name)
            ON CONFLICT DO NOTHING
            """,
            new { InboxMessageId = inboxMessageId, Name = consumerName },
            transaction: transaction);
    }

    private static IEnumerable<IIntegrationEventHandler> GetHandlers(
        Type integrationEventType,
        IServiceProvider serviceProvider,
        Assembly assembly)
    {
        Type[] handlerTypes = assembly.GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IIntegrationEventHandler)))
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType &&
                          i.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>) &&
                          i.GetGenericArguments()[0] == integrationEventType))
            .ToArray();

        List<IIntegrationEventHandler> handlers = [];
        foreach (Type handlerType in handlerTypes)
        {
            object? handler = serviceProvider.GetService(handlerType);
            if (handler is not null)
            {
                handlers.Add((IIntegrationEventHandler)handler);
            }
        }

        return handlers;
    }

    private sealed record InboxMessageResponse(Guid Id, string Content);
}
