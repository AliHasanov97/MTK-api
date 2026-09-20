namespace MTK.Common.Infrastructure.Inbox;

public sealed class InboxMessageConsumer
{
    public Guid InboxMessageId { get; init; }
    public string Name { get; init; } = string.Empty;
}
