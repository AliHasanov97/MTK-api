using MTK.Common.Application.Exporting;
using MTK.Common.Application.Messaging;
using MTK.Common.Application.Storage;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.FileAttachments.Queries.DownloadFileAttachment;

internal sealed class DownloadFileAttachmentQueryHandler : IQueryHandler<DownloadFileAttachmentQuery, ExportFileResult>
{
    private readonly IFileAttachmentRepository _fileAttachmentRepository;
    private readonly IFileStorageService _fileStorageService;

    public DownloadFileAttachmentQueryHandler(
        IFileAttachmentRepository fileAttachmentRepository,
        IFileStorageService fileStorageService)
    {
        _fileAttachmentRepository = fileAttachmentRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<ExportFileResult>> Handle(DownloadFileAttachmentQuery request, CancellationToken cancellationToken)
    {
        var attachment = await _fileAttachmentRepository.GetByIdDefaultAsync(request.FileAttachmentId, cancellationToken);
        if (attachment is null)
        {
            return Result.Failure<ExportFileResult>(new Error("FileAttachment.NotFound", $"Fayl tapılmadı: {request.FileAttachmentId}"));
        }

        await using var stream = await _fileStorageService.DownloadAsync(attachment.ObjectKey, cancellationToken);
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken);

        return new ExportFileResult(memoryStream.ToArray(), attachment.FileName, attachment.ContentType);
    }
}
