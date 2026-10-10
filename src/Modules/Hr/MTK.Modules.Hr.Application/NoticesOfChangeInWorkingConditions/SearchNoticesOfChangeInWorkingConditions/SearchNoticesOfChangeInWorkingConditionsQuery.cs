using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Hr.Application.NoticesOfChangeInWorkingConditions.SearchNoticesOfChangeInWorkingConditions;

public sealed class SearchNoticesOfChangeInWorkingConditionsQuery : IQuery<SearchNoticesOfChangeInWorkingConditionsResponse>
{
    public List<QueryFilter>? Filters { get; set; }
    public SortCriteria? SortCriteria { get; set; }
    public string? SearchTerm { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}