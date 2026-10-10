namespace MTK.Common.Domain.Queries;

public interface ISearchQuery : IKeywordSearchableQuery, IFilterableQuery, ISortableQuery
{
    public int Page { get; set; }
    public int PageSize { get; set; }
}
