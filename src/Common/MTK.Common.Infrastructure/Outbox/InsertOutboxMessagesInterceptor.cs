using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using MTK.Common.Domain.Abstractions;
using MTK.Common.Infrastructure.Serialization;
using Newtonsoft.Json;

namespace MTK.Common.Infrastructure.Outbox;

public sealed class InsertOutboxMessagesInterceptor : SaveChangesInterceptor
{
    private readonly ILogger<InsertOutboxMessagesInterceptor> _logger;

    public InsertOutboxMessagesInterceptor(ILogger<InsertOutboxMessagesInterceptor> logger)
    {
        _logger = logger;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("=== InsertOutboxMessagesInterceptor.SavingChangesAsync called ===");

        if (eventData.Context is not null)
        {
            InsertOutboxMessages(eventData.Context);
        }
        else
        {
            _logger.LogWarning("eventData.Context is null!");
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void InsertOutboxMessages(DbContext context)
    {
        _logger.LogInformation("InsertOutboxMessages - Starting to extract domain events");

        var entries = context.ChangeTracker.Entries<Entity>().ToList();
        _logger.LogInformation("Found {Count} Entity entries in ChangeTracker", entries.Count);

        var domainEvents = entries
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                var events = entity.DomainEvents.ToList();
                _logger.LogInformation("Entity {EntityType} (Id={EntityId}) has {EventCount} domain events",
                    entity.GetType().Name,
                    entity.Id,
                    events.Count);

                foreach (var evt in events)
                {
                    _logger.LogInformation("  - Event: {EventType}, Id={EventId}",
                        evt.GetType().Name,
                        evt.Id);
                }

                entity.ClearDomainEvents();
                return events;
            })
            .ToList();

        _logger.LogInformation("Total domain events collected: {Count}", domainEvents.Count);

        if (!domainEvents.Any())
        {
            _logger.LogInformation("No domain events to process, returning");
            return;
        }

        var outboxMessages = domainEvents
            .Select(domainEvent => new OutboxMessage
            {
                Id = domainEvent.Id,
                Type = domainEvent.GetType().Name,
                Content = JsonConvert.SerializeObject(domainEvent, SerializerSettings.Instance),
                OccurredOnUtc = domainEvent.OccurredOnUtc
            })
            .ToList();

        _logger.LogInformation("Creating {Count} outbox messages", outboxMessages.Count);

        context.Set<OutboxMessage>().AddRange(outboxMessages);

        _logger.LogInformation("Outbox messages added to context");
    }
}
