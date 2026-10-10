using Dapper;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MTK.Common.Application.Auditing;
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

/// <summary>
/// Outbox mesajlarını işləyir.
///
/// - Hər handler üçün icra qeydi <c>outbox_message_consumers</c> cədvəlində saxlanılır: təkrar cəhddə artıq
///   uğurla bitmiş handler yenidən icra olunmur.
/// - Xəta olan mesaj artan fasilə ilə (<see cref="OutboxOptions.RetryBaseDelaySeconds"/> × 4^n) təkrar cəhd olunur;
///   <see cref="OutboxOptions.MaxAttempts"/> cəhddən sonra "ölü" (dead-letter) qeyd edilir (ProcessedOnUtc + Error).
/// - Mesaj olmayan işləmələr log yazmır; mesaj məzmunu yalnız Debug səviyyəsində loglanır.
/// </summary>
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

            if (outboxMessages.Count == 0)
            {
                // Boş işləmə: heç nə loglanmır (hər 5 saniyədə səs-küy yaratmasın)
                await transaction.CommitAsync(context.CancellationToken);
                return;
            }

            logger.LogInformation("{Module} - Processing {Count} outbox message(s)", ModuleName, outboxMessages.Count);

            foreach (OutboxMessageResponse outboxMessage in outboxMessages)
            {
                Exception? exception = null;
                string eventTypeName = "unknown";

                try
                {
                    // MessageId + hadisə növü Information-da, tam məzmun yalnız Debug-da (şəxsi məlumat ola bilər)
                    logger.LogDebug(
                        "{Module} - Outbox message {MessageId} content: {Content}",
                        ModuleName,
                        outboxMessage.Id,
                        outboxMessage.Content);

                    // Use SerializerSettings.Instance with MetadataPropertyHandling.ReadAhead
                    // This tells Newtonsoft.Json to read $type FIRST before deserializing
                    IDomainEvent domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                        outboxMessage.Content,
                        SerializerSettings.Instance)!;

                    eventTypeName = domainEvent.GetType().Name;

                    using IServiceScope scope = serviceScopeFactory.CreateScope();

                    scope.ServiceProvider.GetRequiredService<IAuditActorAccessor>()
                        .SetActorUserId(outboxMessage.TriggeredByUserId);

                    IEnumerable<IDomainEventHandler> handlers = GetHandlers(
                        domainEvent.GetType(),
                        scope.ServiceProvider,
                        HandlerAssembly);

                    foreach (IDomainEventHandler domainEventHandler in handlers)
                    {
                        string consumerName = domainEventHandler.GetType().FullName!;

                        // Təkrar cəhddə artıq uğurla bitmiş handler atlanır
                        if (await ConsumerAlreadyHandledAsync(connection, transaction, outboxMessage.Id, consumerName))
                        {
                            logger.LogDebug(
                                "{Module} - Handler {Handler} already handled message {MessageId}, skipping",
                                ModuleName, consumerName, outboxMessage.Id);
                            continue;
                        }

                        await domainEventHandler.Handle(domainEvent, context.CancellationToken);

                        // Uğurlu icra dərhal qeyd olunur: sonrakı handler xəta versə də bu təkrar işləməsin
                        await InsertConsumerAsync(connection, transaction, outboxMessage.Id, consumerName);
                    }

                    logger.LogInformation(
                        "{Module} - Outbox message {MessageId} ({EventType}) processed",
                        ModuleName, outboxMessage.Id, eventTypeName);
                }
                catch (Exception caughtException)
                {
                    logger.LogError(
                        caughtException,
                        "{Module} - Exception while processing outbox message {MessageId} ({EventType}), attempt {Attempt}/{MaxAttempts}",
                        ModuleName,
                        outboxMessage.Id,
                        eventTypeName,
                        outboxMessage.Attempts + 1,
                        _outboxOptions.MaxAttempts);

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
        // Növbəti cəhd vaxtı gəlməyən (təkrar gözləyən) mesajlar buraxılır
        string sql =
            $"""
             SELECT
                "Id" AS {nameof(OutboxMessageResponse.Id)},
                "Content" AS {nameof(OutboxMessageResponse.Content)},
                "TriggeredByUserId" AS {nameof(OutboxMessageResponse.TriggeredByUserId)},
                "Attempts" AS {nameof(OutboxMessageResponse.Attempts)}
             FROM {Schema}.outbox_messages
             WHERE "ProcessedOnUtc" IS NULL
               AND ("NextAttemptOnUtc" IS NULL OR "NextAttemptOnUtc" <= @Now)
             ORDER BY "OccurredOnUtc"
             LIMIT {_outboxOptions.BatchSize}
             FOR UPDATE
             """;

        IEnumerable<OutboxMessageResponse> outboxMessages = await connection.QueryAsync<OutboxMessageResponse>(
            sql,
            new { Now = dateTimeProvider.UtcNow },
            transaction: transaction);

        return outboxMessages.AsList();
    }

    private async Task UpdateOutboxMessageAsync(
        DbConnection connection,
        DbTransaction transaction,
        OutboxMessageResponse outboxMessage,
        Exception? exception)
    {
        DateTimeOffset now = dateTimeProvider.UtcNow;

        if (exception is null)
        {
            // Uğurlu: işlənmiş kimi qeyd olunur
            await connection.ExecuteAsync(
                $"""
                UPDATE {Schema}.outbox_messages
                SET "ProcessedOnUtc" = @ProcessedOnUtc,
                    "Error" = NULL,
                    "NextAttemptOnUtc" = NULL
                WHERE "Id" = @Id
                """,
                new { outboxMessage.Id, ProcessedOnUtc = now },
                transaction: transaction);
            return;
        }

        int attempts = outboxMessage.Attempts + 1;

        if (attempts >= _outboxOptions.MaxAttempts)
        {
            // Son cəhd də uğursuz: "ölü" (dead-letter) — işlənmiş kimi bağlanır, amma Error dolu qalır
            logger.LogError(
                "{Module} - Outbox message {MessageId} failed {Attempts} times and is marked as dead",
                ModuleName, outboxMessage.Id, attempts);

            await connection.ExecuteAsync(
                $"""
                UPDATE {Schema}.outbox_messages
                SET "ProcessedOnUtc" = @ProcessedOnUtc,
                    "Attempts" = @Attempts,
                    "NextAttemptOnUtc" = NULL,
                    "Error" = @Error
                WHERE "Id" = @Id
                """,
                new { outboxMessage.Id, ProcessedOnUtc = now, Attempts = attempts, Error = exception.ToString() },
                transaction: transaction);
            return;
        }

        // Təkrar cəhd: ProcessedOnUtc boş qalır, növbəti cəhd vaxtı artan fasilə ilə təyin olunur
        double delaySeconds = _outboxOptions.RetryBaseDelaySeconds * Math.Pow(4, attempts - 1);
        DateTimeOffset nextAttempt = now.AddSeconds(delaySeconds);

        await connection.ExecuteAsync(
            $"""
            UPDATE {Schema}.outbox_messages
            SET "Attempts" = @Attempts,
                "NextAttemptOnUtc" = @NextAttemptOnUtc,
                "Error" = @Error
            WHERE "Id" = @Id
            """,
            new { outboxMessage.Id, Attempts = attempts, NextAttemptOnUtc = nextAttempt, Error = exception.ToString() },
            transaction: transaction);
    }

    private async Task<bool> ConsumerAlreadyHandledAsync(
        DbConnection connection,
        DbTransaction transaction,
        Guid outboxMessageId,
        string consumerName)
    {
        return await connection.ExecuteScalarAsync<bool>(
            $"""
            SELECT EXISTS (
                SELECT 1 FROM {Schema}.outbox_message_consumers
                WHERE "OutboxMessageId" = @OutboxMessageId AND "Name" = @Name)
            """,
            new { OutboxMessageId = outboxMessageId, Name = consumerName },
            transaction: transaction);
    }

    private async Task InsertConsumerAsync(
        DbConnection connection,
        DbTransaction transaction,
        Guid outboxMessageId,
        string consumerName)
    {
        await connection.ExecuteAsync(
            $"""
            INSERT INTO {Schema}.outbox_message_consumers ("OutboxMessageId", "Name")
            VALUES (@OutboxMessageId, @Name)
            ON CONFLICT DO NOTHING
            """,
            new { OutboxMessageId = outboxMessageId, Name = consumerName },
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

    private sealed record OutboxMessageResponse(Guid Id, string Content, Guid? TriggeredByUserId, int Attempts);
}
