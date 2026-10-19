using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Communications.Domain.Articles;

namespace MTK.Modules.Communications.Application.Articles.Queries.GetLatestArticles;

internal sealed class GetLatestArticlesQueryHandler
    : IQueryHandler<GetLatestArticlesQuery, List<ArticleResponse>>
{
    private readonly IArticleRepository _articleRepository;

    public GetLatestArticlesQueryHandler(IArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public async Task<Result<List<ArticleResponse>>> Handle(
        GetLatestArticlesQuery request,
        CancellationToken cancellationToken)
    {
        var articles = await _articleRepository.GetLatestPublishedAsync(
            request.Count,
            cancellationToken);

        var response = articles.Select(a => new ArticleResponse(
            a.Id,
            a.Title,
            a.Summary,
            a.Content,
            a.Status.ToString(),
            a.PublishedAt,
            a.CreatedAt,
            a.ViewCount,
            a.CreatedBy.FullName,
            a.FileAttachments.Select(f => new FileAttachmentResponse(
                f.Id,
                f.FileName,
                f.MimeType,
                f.IsPublic)).ToList())).ToList();

        return Result.Success(response);
    }
}
