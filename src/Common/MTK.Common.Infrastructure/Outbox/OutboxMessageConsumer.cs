namespace MTK.Common.Infrastructure.Outbox;

public sealed class OutboxMessageConsumer
{
    public Guid OutboxMessageId { get; init; }
    public string Name { get; init; } = string.Empty;
}
