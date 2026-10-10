using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Hr.Application.VacationCompensationApplications.SearchVacationCompensationApplications;

public sealed class SearchVacationCompensationApplicationsQuery : IQuery<SearchVacationCompensationApplicationsResponse>
{
    public List<QueryFilter>? Filters { get; set; }
    public SortCriteria? SortCriteria { get; set; }
    public string? SearchTerm { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}