using MTK.Common.Application.Messaging;
using MTK.Common.Application.Storage;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Communications.Domain.FileAttachments;

namespace MTK.Modules.Communications.Application.FileAttachments.DownloadFileAttachment;

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
        var attachment = await _fileAttachmentRepository.GetByIdDefaultAsync(request.Id, cancellationToken);
        if (attachment is null)
        {
            return Result.Failure<DownloadFileAttachmentResponse>(FileAttachmentErrors.NotFound);
        }

        var stream = await _fileStorageService.DownloadAsync(
            FileAttachmentStorageKey.For(attachment.Id),
            cancellationToken);

        return Result.Success(new DownloadFileAttachmentResponse
        {
            FileStream = stream,
            FileName = attachment.FileName,
            ContentType = GetContentType(attachment.MimeType),
            IsPublic = attachment.IsPublic
        });
    }

    private static string GetContentType(string extension) => extension.ToLowerInvariant() switch
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
