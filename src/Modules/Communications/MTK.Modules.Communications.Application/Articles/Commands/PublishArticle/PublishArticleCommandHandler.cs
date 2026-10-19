using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Communications.Application.Abstractions.Data;
using MTK.Modules.Communications.Domain.Articles;

namespace MTK.Modules.Communications.Application.Articles.Commands.PublishArticle;

internal sealed class PublishArticleCommandHandler : ICommandHandler<PublishArticleCommand>
{
    private readonly IArticleRepository _articleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PublishArticleCommandHandler(
        IArticleRepository articleRepository,
        IUnitOfWork unitOfWork)
    {
        _articleRepository = articleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        PublishArticleCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var article = await _articleRepository.GetByIdDefaultAsync(
                request.Id,
                cancellationToken);

            if (article is null)
            {
                return Result.Failure(ArticleErrors.NotFound);
            }

            article.Publish();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception)
        {
            return Result.Failure(ArticleErrors.PublishFailed);
        }
    }
}
