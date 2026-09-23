using System.Text.Json.Serialization;

namespace MTK.Modules.Identity.Infrastructure.Keycloak;

internal sealed class UserRepresentationModel
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("username")]
    public string Username { get; init; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; init; } = string.Empty;

    [JsonPropertyName("firstName")]
    public string FirstName { get; init; } = string.Empty;

    [JsonPropertyName("lastName")]
    public string LastName { get; init; } = string.Empty;

    [JsonPropertyName("enabled")]
    public bool Enabled { get; init; } = true;

    [JsonPropertyName("emailVerified")]
    public bool EmailVerified { get; init; } = false;

    [JsonPropertyName("credentials")]
    public List<CredentialRepresentationModel>? Credentials { get; init; }
}
