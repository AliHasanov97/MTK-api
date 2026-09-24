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
        logger.LogInformation("{Module} - Beginning to process inbox messages", ModuleName);

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

            foreach (InboxMessageResponse inboxMessage in inboxMessages)
            {
                Exception? exception = null;

                try
                {
                    logger.LogInformation(
                        "{Module} - Processing inbox message {MessageId}. Content: {Content}",
                        ModuleName,
                        inboxMessage.Id,
                        inboxMessage.Content);

                    // Use SerializerSettings.Instance with MetadataPropertyHandling.ReadAhead
                    // This tells Newtonsoft.Json to read $type FIRST before deserializing
                    IIntegrationEvent integrationEvent = JsonConvert.DeserializeObject<IIntegrationEvent>(
                        inboxMessage.Content,
                        SerializerSettings.Instance)!;

                    logger.LogInformation(
                        "{Module} - Deserialized integration event of type {EventType}",
                        ModuleName,
                        integrationEvent.GetType().Name);

                    using IServiceScope scope = serviceScopeFactory.CreateScope();

                    IEnumerable<IIntegrationEventHandler> handlers = GetHandlers(
                        integrationEvent.GetType(),
                        scope.ServiceProvider,
                        HandlerAssembly);

                    logger.LogInformation(
                        "{Module} - Found {HandlerCount} handlers for event type {EventType}",
                        ModuleName,
                        handlers.Count(),
                        integrationEvent.GetType().Name);

                    foreach (IIntegrationEventHandler integrationEventHandler in handlers)
                    {
                        logger.LogInformation(
                            "{Module} - Executing handler {HandlerType}",
                            ModuleName,
                            integrationEventHandler.GetType().Name);

                        await integrationEventHandler.Handle(integrationEvent, context.CancellationToken);

                        logger.LogInformation(
                            "{Module} - Handler {HandlerType} completed successfully",
                            ModuleName,
                            integrationEventHandler.GetType().Name);
                    }
                }
                catch (Exception caughtException)
                {
                    logger.LogError(
                        caughtException,
                        "{Module} - Exception while processing inbox message {MessageId}. Content: {Content}",
                        ModuleName,
                        inboxMessage.Id,
                        inboxMessage.Content);

                    exception = caughtException;
                }

                await UpdateInboxMessageAsync(connection, transaction, inboxMessage, exception);
            }

            await transaction.CommitAsync(context.CancellationToken);
            logger.LogInformation("{Module} - Completed processing inbox messages", ModuleName);
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
