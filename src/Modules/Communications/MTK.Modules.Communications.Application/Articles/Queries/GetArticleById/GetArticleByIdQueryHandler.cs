using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Communications.Domain.Articles;

namespace MTK.Modules.Communications.Application.Articles.Queries.GetArticleById;

internal sealed class GetArticleByIdQueryHandler
    : IQueryHandler<GetArticleByIdQuery, ArticleResponse>
{
    private readonly IArticleRepository _articleRepository;

    public GetArticleByIdQueryHandler(IArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public async Task<Result<ArticleResponse>> Handle(
        GetArticleByIdQuery request,
        CancellationToken cancellationToken)
    {
        var article = await _articleRepository.GetByIdAndIncrementViewAsync(
            request.Id,
            cancellationToken);

        if (article is null)
        {
            return Result.Failure<ArticleResponse>(ArticleErrors.NotFound);
        }

        var response = new ArticleResponse(
            article.Id,
            article.Title,
            article.Summary,
            article.Content,
            article.Status.ToString(),
            article.PublishedAt,
            article.CreatedAt,
            article.ViewCount,
            article.CreatedBy.FullName,
            article.FileAttachments.Select(f => new FileAttachmentResponse(
                f.Id,
                f.FileName,
                f.MimeType,
                f.IsPublic)).ToList());

        return Result.Success(response);
    }
}
