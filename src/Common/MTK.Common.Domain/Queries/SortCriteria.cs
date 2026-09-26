namespace MTK.Common.Domain.Queries;

public class SortCriteria
{
    public string ColumnName { get; set; } = string.Empty;
    public SortDirection Direction { get; set; } = SortDirection.Ascending;
}
