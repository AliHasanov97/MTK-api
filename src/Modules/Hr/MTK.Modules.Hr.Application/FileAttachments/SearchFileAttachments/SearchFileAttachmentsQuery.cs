using MTK.Common.Domain.Queries;
using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.FileAttachments.SearchFileAttachments;

public class SearchFileAttachmentsQuery : IQuery<SearchFileAttachmentsResponse>, ISearchQuery
{
    public string? SearchTerm { get; set; }
    public List<QueryFilter>? Filters { get; set; }
    public SortCriteria? SortCriteria { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
