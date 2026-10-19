using MTK.Common.Application.Messaging;
using MTK.Common.Application.Storage;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Communications.Application.Abstractions.Data;
using MTK.Modules.Communications.Domain.Articles;
using MTK.Modules.Communications.Domain.FileAttachments;

namespace MTK.Modules.Communications.Application.FileAttachments.AddFileAttachment;

internal sealed class AddFileAttachmentCommandHandler
    : ICommandHandler<AddFileAttachmentCommand, AddFileAttachmentResponse>
{
    private const long MaxFileSizeBytes = 20 * 1024 * 1024;

    private readonly IArticleRepository _articleRepository;
    private readonly IFileAttachmentRepository _fileAttachmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public AddFileAttachmentCommandHandler(
        IArticleRepository articleRepository,
        IFileAttachmentRepository fileAttachmentRepository,
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorageService)
    {
        _articleRepository = articleRepository;
        _fileAttachmentRepository = fileAttachmentRepository;
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<AddFileAttachmentResponse>> Handle(
        AddFileAttachmentCommand request,
        CancellationToken cancellationToken)
    {
        if (request.File is null || request.File.Length == 0)
        {
            return Result.Failure<AddFileAttachmentResponse>(FileAttachmentErrors.FileRequired);
        }

        if (request.File.Length > MaxFileSizeBytes)
        {
            return Result.Failure<AddFileAttachmentResponse>(FileAttachmentErrors.TooLarge);
        }

        var article = await _articleRepository.GetByIdDefaultAsync(request.ArticleId, cancellationToken);
        if (article is null)
        {
            return Result.Failure<AddFileAttachmentResponse>(FileAttachmentErrors.ArticleNotFound);
        }

        string? uploadedPath = null;
        try
        {
            var fileId = Guid.NewGuid();

            await using (var stream = request.File.OpenReadStream())
            {
                uploadedPath = await _fileStorageService.UploadAsync(
                    stream,
                    FileAttachmentStorageKey.For(fileId),
                    request.File.ContentType,
                    cancellationToken);
            }

            // Digər modullardakı kimi MimeType sahəsində fayl uzantısı saxlanılır (pdf, docx, png…).
            var extension = Path.GetExtension(request.File.FileName).TrimStart('.').ToLowerInvariant();

            var attachment = FileAttachment.Create(
                fileId,
                request.File.FileName,
                extension,
                request.IsPublic,
                article.Id);

            await _fileAttachmentRepository.AddAsync(attachment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(new AddFileAttachmentResponse(
                attachment.Id,
                attachment.ArticleId,
                attachment.FileName,
                attachment.MimeType,
                attachment.IsPublic));
        }
        catch (Exception)
        {
            if (!string.IsNullOrEmpty(uploadedPath))
            {
                await _fileStorageService.DeleteAsync(uploadedPath);
            }

            return Result.Failure<AddFileAttachmentResponse>(FileAttachmentErrors.CreateFailed);
        }
    }
}
