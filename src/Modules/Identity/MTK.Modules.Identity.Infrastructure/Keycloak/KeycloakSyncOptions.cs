namespace MTK.Modules.Identity.Infrastructure.Keycloak;

public sealed class KeycloakSyncOptions
{
    public int IntervalInMinutes { get; init; } = 30;
    public int BatchSize { get; init; } = 100;
}
