namespace MTK.Common.Domain.Queries;

public class QueryFilter
{
    public string ColumnName { get; set; } = string.Empty;
    public QueryComparisonType Comparison { get; set; }
    /// <summary>IsNull/IsNotNull müqayisələrində tələb olunmur.</summary>
    public object? Value { get; set; }
}
