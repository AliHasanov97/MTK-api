namespace MTK.Common.Infrastructure.Outbox;

public sealed class OutboxOptions
{
    public int IntervalInSeconds { get; init; }
    public int BatchSize { get; init; }

    // Xəta olan mesajın maksimum cəhd sayı (sonra dead-letter) və ilk təkrar cəhd fasiləsi (saniyə);
    // sonrakı fasilələr 4 dəfə artır: 30s, 2m, 8m, 32m …
    public int MaxAttempts { get; init; } = 5;
    public int RetryBaseDelaySeconds { get; init; } = 30;
}
