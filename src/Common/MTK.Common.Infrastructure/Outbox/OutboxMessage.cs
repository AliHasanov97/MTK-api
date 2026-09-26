namespace MTK.Common.Infrastructure.Outbox;

public sealed class OutboxMessage
{
    public Guid Id { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public DateTimeOffset OccurredOnUtc { get; init; }
    public DateTimeOffset? ProcessedOnUtc { get; init; }
    public string? Error { get; init; }
}
