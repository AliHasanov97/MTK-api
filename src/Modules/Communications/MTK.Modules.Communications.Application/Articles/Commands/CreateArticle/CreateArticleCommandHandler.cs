using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Communications.Application.Abstractions.Authentication;
using MTK.Modules.Communications.Application.Abstractions.Data;
using MTK.Modules.Communications.Domain.Articles;

namespace MTK.Modules.Communications.Application.Articles.Commands.CreateArticle;

internal sealed class CreateArticleCommandHandler
    : ICommandHandler<CreateArticleCommand, CreateArticleResponse>
{
    private readonly IArticleRepository _articleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public CreateArticleCommandHandler(
        IArticleRepository articleRepository,
        IUnitOfWork unitOfWork,
        IUserContext userContext)
    {
        _articleRepository = articleRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result<CreateArticleResponse>> Handle(
        CreateArticleCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var article = Article.Create(
                request.Title,
                request.Summary,
                request.Content,
                _userContext.UserId);

            await _articleRepository.AddAsync(article, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new CreateArticleResponse(
                article.Id,
                article.Title,
                article.Summary,
                article.Content,
                article.Status.ToString(),
                article.CreatedAt);

            return Result.Success(response);
        }
        catch (Exception)
        {
            return Result.Failure<CreateArticleResponse>(ArticleErrors.CreateFailed);
        }
    }
}
