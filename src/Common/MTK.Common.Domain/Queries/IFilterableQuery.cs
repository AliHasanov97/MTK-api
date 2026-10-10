namespace MTK.Common.Domain.Queries;

public interface IFilterableQuery
{
    public List<QueryFilter>? Filters { get; set; }
}
