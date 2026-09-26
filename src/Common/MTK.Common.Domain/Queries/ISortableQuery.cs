namespace MTK.Common.Domain.Queries;

public interface ISortableQuery
{
    public SortCriteria? SortCriteria { get; set; }
}
