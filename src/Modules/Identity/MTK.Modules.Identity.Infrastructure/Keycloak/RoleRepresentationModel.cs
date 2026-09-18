using System.Text.Json.Serialization;

namespace MTK.Modules.Identity.Infrastructure.Keycloak;

internal sealed class RoleRepresentationModel
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; init; }
}
