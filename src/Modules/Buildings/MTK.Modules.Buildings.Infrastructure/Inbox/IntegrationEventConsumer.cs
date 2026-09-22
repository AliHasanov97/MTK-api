using Dapper;
using MassTransit;
using MTK.Common.Application.Data;
using MTK.Common.Application.EventBus;
using MTK.Common.Infrastructure.Inbox;
using Newtonsoft.Json;
using System.Data.Common;

namespace MTK.Modules.Buildings.Infrastructure.Inbox;

internal sealed class IntegrationEventConsumer<TIntegrationEvent>(
    IDbConnectionFactory dbConnectionFactory)
    : IConsumer<TIntegrationEvent>
    where TIntegrationEvent : class, IIntegrationEvent
{
    public async Task Consume(ConsumeContext<TIntegrationEvent> context)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        TIntegrationEvent integrationEvent = context.Message;

        var inboxMessage = new InboxMessage
        {
            Id = integrationEvent.IntegrationEventId,
            Type = integrationEvent.GetType().Name,
            Content = JsonConvert.SerializeObject(integrationEvent, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            }),
            OccurredOnUtc = integrationEvent.OccurredOnUtc
        };

        const string sql =
            """
            INSERT INTO buildings.inbox_messages(id, type, content, occurred_on_utc)
            VALUES (@Id, @Type, @Content::jsonb, @OccurredOnUtc)
            ON CONFLICT (id) DO NOTHING
            """;

        await connection.ExecuteAsync(sql, inboxMessage);
    }
}
