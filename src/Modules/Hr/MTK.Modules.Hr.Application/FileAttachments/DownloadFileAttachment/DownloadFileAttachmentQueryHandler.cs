using MTK.Common.Application.Storage;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.FileAttachments;

namespace MTK.Modules.Hr.Application.FileAttachments.DownloadFileAttachment;

internal sealed class DownloadFileAttachmentQueryHandler
    : IQueryHandler<DownloadFileAttachmentQuery, DownloadFileAttachmentResponse>
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

    public async Task<Result<DownloadFileAttachmentResponse>> Handle(
        DownloadFileAttachmentQuery request,
        CancellationToken cancellationToken)
    {
        var fileAttachment = await _fileAttachmentRepository.GetByIdDefaultAsync(request.Id, cancellationToken);

        if (fileAttachment == null)
        {
            return Result.Failure<DownloadFileAttachmentResponse>(FileAttachmentErrorMessages.NotFound);
        }

        var fileStream = await _fileStorageService.DownloadAsync(
            FileAttachmentStorageKey.For(fileAttachment.Id), cancellationToken);

        var response = new DownloadFileAttachmentResponse
        {
            FileStream = fileStream,
            FileName = fileAttachment.FileName,
            ContentType = GetContentType(fileAttachment.MimeType)
        };

        return Result.Success(response);
    }

    private static string GetContentType(string extension)
    {
        return extension.ToLowerInvariant() switch
        {
            "pdf" => "application/pdf",
            "doc" => "application/msword",
            "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "xls" => "application/vnd.ms-excel",
            "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "png" => "image/png",
            "jpg" or "jpeg" => "image/jpeg",
            "gif" => "image/gif",
            "txt" => "text/plain",
            "zip" => "application/zip",
            "rar" => "application/x-rar-compressed",
            _ => "application/octet-stream"
        };
    }
}
