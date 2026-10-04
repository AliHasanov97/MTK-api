using MTK.Common.Application.Messaging;
using MTK.Common.Application.Storage;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Domain.FileAttachments;

namespace MTK.Modules.Identity.Application.FileAttachments.Queries.GetFileAttachmentDownloadLink;

internal sealed class GetFileAttachmentDownloadLinkQueryHandler : IQueryHandler<GetFileAttachmentDownloadLinkQuery, string>
{
    private static readonly TimeSpan LinkValidity = TimeSpan.FromMinutes(15);

    private readonly IFileAttachmentRepository _fileAttachmentRepository;
    private readonly IFileStorageService _fileStorageService;

    public GetFileAttachmentDownloadLinkQueryHandler(
        IFileAttachmentRepository fileAttachmentRepository,
        IFileStorageService fileStorageService)
    {
        _fileAttachmentRepository = fileAttachmentRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<string>> Handle(GetFileAttachmentDownloadLinkQuery request, CancellationToken cancellationToken)
    {
        var attachment = await _fileAttachmentRepository.GetByIdDefaultAsync(request.FileAttachmentId, cancellationToken);
        if (attachment is null)
        {
            return Result.Failure<string>(new Error("FileAttachment.NotFound", $"Fayl tapılmadı: {request.FileAttachmentId}"));
        }

        return _fileStorageService.GetPresignedDownloadUrl(attachment.ObjectKey, LinkValidity);
    }
}
