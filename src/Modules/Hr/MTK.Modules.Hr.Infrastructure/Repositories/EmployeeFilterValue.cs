using System.Text.Json;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal static class EmployeeFilterValue
{
    /// <summary>QueryFilter.Value sorğu gövdəsindən JsonElement kimi gəlir; Guid-ə çevirir.</summary>
    public static Guid ToGuid(object? value) => value switch
    {
        Guid g => g,
        JsonElement { ValueKind: JsonValueKind.String } je when Guid.TryParse(je.GetString(), out var g) => g,
        string s when Guid.TryParse(s, out var g) => g,
        _ => Guid.Empty
    };
}
