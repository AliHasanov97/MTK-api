namespace MTK.Modules.Identity.Infrastructure.Authentication;

public sealed class KeycloakOptions
{
    public string Realm { get; init; } = string.Empty;
    public string OpenIdUrl { get; init; } = string.Empty;
    public string TokenUrl { get; init; } = string.Empty;
    public string AuthClientId { get; init; } = string.Empty;
    public string AuthClientSecret { get; init; } = string.Empty;
    public string AdminClientId { get; init; } = string.Empty;
    public string AdminClientSecret { get; init; } = string.Empty;
    public string AdminUrl { get; init; } = string.Empty;
    public string AuthorizationUrl { get; init; } = string.Empty;
}
