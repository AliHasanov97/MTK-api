using System.Text.Json.Serialization;

namespace MTK.Modules.Identity.Infrastructure.Keycloak;

internal sealed class CredentialRepresentationModel
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = "password";

    [JsonPropertyName("value")]
    public string Value { get; init; } = string.Empty;

    [JsonPropertyName("temporary")]
    public bool Temporary { get; init; } = false;
}
