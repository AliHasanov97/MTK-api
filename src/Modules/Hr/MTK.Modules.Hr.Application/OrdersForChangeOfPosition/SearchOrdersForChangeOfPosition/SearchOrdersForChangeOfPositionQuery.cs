using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Hr.Application.OrdersForChangeOfPosition.SearchOrdersForChangeOfPosition;

public sealed class SearchOrdersForChangeOfPositionQuery : IQuery<SearchOrdersForChangeOfPositionResponse>
{
    public List<QueryFilter>? Filters { get; set; }
    public SortCriteria? SortCriteria { get; set; }
    public string? SearchTerm { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}
