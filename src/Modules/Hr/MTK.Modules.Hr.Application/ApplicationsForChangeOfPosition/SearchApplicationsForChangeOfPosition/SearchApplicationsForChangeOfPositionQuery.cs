using MTK.Common.Presentation.Responses;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Hr.Application.ApplicationsForChangeOfPosition.SearchApplicationsForChangeOfPosition;

public sealed class SearchApplicationsForChangeOfPositionQuery : IQuery<SearchApplicationsForChangeOfPositionResponse>
{
    public List<QueryFilter>? Filters { get; set; }
    public SortCriteria? SortCriteria { get; set; }
    public string? SearchTerm { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}
