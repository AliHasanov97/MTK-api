using Dapper;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MTK.Common.Application.Data;
using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Common.Infrastructure.Serialization;
using Newtonsoft.Json;
using Npgsql;
using Quartz;
using System.Data.Common;
using System.Reflection;

namespace MTK.Common.Infrastructure.Outbox;

[DisallowConcurrentExecution]
public abstract class ProcessOutboxJobBase(
    IDbConnectionFactory dbConnectionFactory,
    IServiceScopeFactory serviceScopeFactory,
    IDateTimeProvider dateTimeProvider,
    IOptions<OutboxOptions> outboxOptions,
    ILogger logger) : IJob
{
    protected abstract string ModuleName { get; }
    protected abstract string Schema { get; }
    protected abstract Assembly HandlerAssembly { get; }

    private readonly OutboxOptions _outboxOptions = outboxOptions.Value;

    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("{Module} - Beginning to process outbox messages", ModuleName);

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
            IReadOnlyList<OutboxMessageResponse> outboxMessages =
                await GetOutboxMessagesAsync(connection, transaction);

            foreach (OutboxMessageResponse outboxMessage in outboxMessages)
            {
                Exception? exception = null;

                try
                {
                    logger.LogInformation(
                        "{Module} - Processing outbox message {MessageId}. Content: {Content}",
                        ModuleName,
                        outboxMessage.Id,
                        outboxMessage.Content);

                    // Use SerializerSettings.Instance with MetadataPropertyHandling.ReadAhead
                    // This tells Newtonsoft.Json to read $type FIRST before deserializing
                    IDomainEvent domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                        outboxMessage.Content,
                        SerializerSettings.Instance)!;

                    logger.LogInformation(
                        "{Module} - Deserialized domain event of type {EventType}",
                        ModuleName,
                        domainEvent.GetType().Name);

                    using IServiceScope scope = serviceScopeFactory.CreateScope();

                    IEnumerable<IDomainEventHandler> handlers = GetHandlers(
                        domainEvent.GetType(),
                        scope.ServiceProvider,
                        HandlerAssembly);

                    logger.LogInformation(
                        "{Module} - Found {HandlerCount} handlers for event type {EventType}",
                        ModuleName,
                        handlers.Count(),
                        domainEvent.GetType().Name);

                    foreach (IDomainEventHandler domainEventHandler in handlers)
                    {
                        logger.LogInformation(
                            "{Module} - Executing handler {HandlerType}",
                            ModuleName,
                            domainEventHandler.GetType().Name);

                        await domainEventHandler.Handle(domainEvent, context.CancellationToken);

                        logger.LogInformation(
                            "{Module} - Handler {HandlerType} completed successfully",
                            ModuleName,
                            domainEventHandler.GetType().Name);
                    }
                }
                catch (Exception caughtException)
                {
                    logger.LogError(
                        caughtException,
                        "{Module} - Exception while processing outbox message {MessageId}. Content: {Content}",
                        ModuleName,
                        outboxMessage.Id,
                        outboxMessage.Content);

                    exception = caughtException;
                }

                if (exception is TransportUnavailableException)
                {
                    logger.LogWarning(
                        "{Module} - Transport unavailable for message {MessageId}, will retry on next run",
                        ModuleName,
                        outboxMessage.Id);
                    continue;
                }

                await UpdateOutboxMessageAsync(connection, transaction, outboxMessage, exception);
            }

            await transaction.CommitAsync(context.CancellationToken);
            logger.LogInformation("{Module} - Completed processing outbox messages", ModuleName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Module} - Error processing outbox messages, rolling back transaction", ModuleName);
            await transaction.RollbackAsync(context.CancellationToken);
            throw;
        }
    }

    private async Task<IReadOnlyList<OutboxMessageResponse>> GetOutboxMessagesAsync(
        DbConnection connection,
        DbTransaction transaction)
    {
        string sql =
            $"""
             SELECT
                "Id" AS {nameof(OutboxMessageResponse.Id)},
                "Content" AS {nameof(OutboxMessageResponse.Content)}
             FROM {Schema}.outbox_messages
             WHERE "ProcessedOnUtc" IS NULL
             ORDER BY "OccurredOnUtc"
             LIMIT {_outboxOptions.BatchSize}
             FOR UPDATE
             """;

        IEnumerable<OutboxMessageResponse> outboxMessages = await connection.QueryAsync<OutboxMessageResponse>(
            sql,
            transaction: transaction);

        return outboxMessages.AsList();
    }

    private async Task UpdateOutboxMessageAsync(
        DbConnection connection,
        DbTransaction transaction,
        OutboxMessageResponse outboxMessage,
        Exception? exception)
    {
        string sql =
            $"""
            UPDATE {Schema}.outbox_messages
            SET "ProcessedOnUtc" = @ProcessedOnUtc,
                "Error" = @Error
            WHERE "Id" = @Id
            """;

        await connection.ExecuteAsync(
            sql,
            new
            {
                outboxMessage.Id,
                ProcessedOnUtc = dateTimeProvider.UtcNow,
                Error = exception?.ToString()
            },
            transaction: transaction);
    }

    private static IEnumerable<IDomainEventHandler> GetHandlers(
        Type domainEventType,
        IServiceProvider serviceProvider,
        Assembly assembly)
    {
        Type[] handlerTypes = assembly.GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IDomainEventHandler)))
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType &&
                          i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>) &&
                          i.GetGenericArguments()[0] == domainEventType))
            .ToArray();

        List<IDomainEventHandler> handlers = [];
        foreach (Type handlerType in handlerTypes)
        {
            object? handler = serviceProvider.GetService(handlerType);
            if (handler is not null)
            {
                handlers.Add((IDomainEventHandler)handler);
            }
        }

        return handlers;
    }

    private sealed record OutboxMessageResponse(Guid Id, string Content);
}
