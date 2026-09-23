using System.Text.Json.Serialization;

namespace MTK.Modules.Identity.Infrastructure.Keycloak;

internal sealed class GroupRepresentationModel
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("parentId")]
    public string? ParentId { get; set; }

    [JsonPropertyName("path")]
    public string? Path { get; set; }

    [JsonPropertyName("attributes")]
    public Dictionary<string, List<string>>? Attributes { get; set; }

    [JsonPropertyName("realmRoles")]
    public List<string>? RealmRoles { get; set; }

    [JsonPropertyName("clientRoles")]
    public Dictionary<string, List<string>>? ClientRoles { get; set; }

    [JsonPropertyName("subGroups")]
    public List<GroupRepresentationModel>? SubGroups { get; set; }
}
