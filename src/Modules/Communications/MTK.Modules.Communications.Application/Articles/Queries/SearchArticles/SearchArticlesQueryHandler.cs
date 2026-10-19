using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Communications.Domain.Articles;

namespace MTK.Modules.Communications.Application.Articles.Queries.SearchArticles;

internal sealed class SearchArticlesQueryHandler(IArticleRepository repository)
    : IQueryHandler<SearchArticlesQuery, SearchArticlesResponse>
{
    public async Task<Result<SearchArticlesResponse>> Handle(
        SearchArticlesQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var articles = await repository.SearchAsync(
                request.Filters,
                request.SortCriteria,
                request.SearchTerm,
                request.Page,
                request.PageSize,
                cancellationToken);

            var totalCount = await repository.CountAsync(
                request.Filters,
                request.SortCriteria,
                request.SearchTerm,
                cancellationToken);

            var data = articles.Select(a => new SearchArticlesResponseItem
            {
                Id = a.Id,
                Title = a.Title,
                Summary = a.Summary,
                Status = a.Status.ToString(),
                PublishedAt = a.PublishedAt,
                CreatedAt = a.CreatedAt,
                ViewCount = a.ViewCount,
                CreatedByFullName = a.CreatedBy.FullName,
                FileAttachmentCount = a.FileAttachments.Count
            }).ToList();

            return Result.Success(new SearchArticlesResponse(
                data,
                totalCount,
                request.Page ?? 1,
                request.PageSize ?? 10));
        }
        catch (Exception)
        {
            throw;
        }
    }
}
