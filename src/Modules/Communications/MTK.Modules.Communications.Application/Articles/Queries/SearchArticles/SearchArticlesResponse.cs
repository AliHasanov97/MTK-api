using MTK.Common.Domain.Queries;

namespace MTK.Modules.Communications.Application.Articles.Queries.SearchArticles;

public sealed class SearchArticlesResponse(
    List<SearchArticlesResponseItem> data,
    int totalCount,
    int page,
    int pageSize)
    : PagedListResponse<SearchArticlesResponseItem>(data, totalCount, page, pageSize);

public sealed class SearchArticlesResponseItem
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Summary { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTimeOffset? PublishedAt { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public int ViewCount { get; init; }
    public string CreatedByFullName { get; init; } = string.Empty;
    public int FileAttachmentCount { get; init; }
}
