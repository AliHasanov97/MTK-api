using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Hr.Application.VacationReturnApplications.SearchVacationReturnApplications;

public sealed class SearchVacationReturnApplicationsQuery : ISearchQuery, IQuery<SearchVacationReturnApplicationsResponse>
{
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 0;
    public int PageSize { get; set; } = 10;
    public List<QueryFilter>? Filters { get; set; }
    public SortCriteria? SortCriteria { get; set; }
}