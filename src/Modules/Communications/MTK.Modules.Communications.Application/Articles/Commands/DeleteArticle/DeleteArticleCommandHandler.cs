using MTK.Common.Application.Messaging;
using MTK.Common.Application.Storage;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Communications.Application.Abstractions.Data;
using MTK.Modules.Communications.Application.FileAttachments;
using MTK.Modules.Communications.Domain.Articles;

namespace MTK.Modules.Communications.Application.Articles.Commands.DeleteArticle;

internal sealed class DeleteArticleCommandHandler : ICommandHandler<DeleteArticleCommand>
{
    private readonly IArticleRepository _articleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public DeleteArticleCommandHandler(
        IArticleRepository articleRepository,
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorageService)
    {
        _articleRepository = articleRepository;
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result> Handle(
        DeleteArticleCommand request,
        CancellationToken cancellationToken)
    {
        List<Guid> attachmentIds;

        try
        {
            var article = await _articleRepository.GetByIdDefaultAsync(
                request.Id,
                cancellationToken);

            if (article is null)
            {
                return Result.Failure(ArticleErrors.NotFound);
            }

            // Xəbərin faylları DB-də cascade ilə silinir; storage-dəki obyektlər isə özləri silinmir.
            attachmentIds = article.FileAttachments.Select(f => f.Id).ToList();

            await _articleRepository.DeleteAsync(article, null, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception)
        {
            return Result.Failure(ArticleErrors.DeleteFailed);
        }

        // DB silindikdən SONRA storage təmizlənir: bu addım uğursuz olsa belə xəbər artıq silinmiş sayılır,
        // yalnız yetim fayl qala bilər (xəbərin özü silinib, istifadəçiyə xəta göstərilmir).
        foreach (var attachmentId in attachmentIds)
        {
            try
            {
                await _fileStorageService.DeleteAsync(FileAttachmentStorageKey.For(attachmentId), cancellationToken);
            }
            catch (Exception)
            {
                // best-effort
            }
        }

        return Result.Success();
    }
}
