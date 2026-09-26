namespace MTK.Common.Domain.Queries;

public class QueryFilter
{
    public string ColumnName { get; set; } = string.Empty;
    public QueryComparisonType Comparison { get; set; }
    public object Value { get; set; } = null!;
}
