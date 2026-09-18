using System.Text.Json.Serialization;

namespace MTK.Modules.Identity.Infrastructure.Keycloak;

internal sealed record AuthorizationToken(
    [property: JsonPropertyName("access_token")] string AccessToken);
