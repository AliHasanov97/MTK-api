using MTK.Common.Application.Messaging;
using MTK.Common.Application.Storage;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.FileAttachments.Commands.DeleteFileAttachment;

internal sealed class DeleteFileAttachmentCommandHandler : ICommandHandler<DeleteFileAttachmentCommand>
{
    private readonly IFileAttachmentRepository _fileAttachmentRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteFileAttachmentCommandHandler(
        IFileAttachmentRepository fileAttachmentRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork)
    {
        _fileAttachmentRepository = fileAttachmentRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteFileAttachmentCommand request, CancellationToken cancellationToken)
    {
        var attachment = await _fileAttachmentRepository.GetByIdDefaultAsync(request.FileAttachmentId, cancellationToken);
        if (attachment is null)
        {
            return Result.Failure(new Error("FileAttachment.NotFound", $"Fayl tapılmadı: {request.FileAttachmentId}"));
        }

        if (attachment.UploadedByUserId != request.RequestedByUserId)
        {
            return Result.Failure(new Error(
                "FileAttachment.NotUploader",
                "Yalnız bu sənədi yükləyən şəxs onu silə bilər"));
        }

        await _fileStorageService.DeleteAsync(attachment.ObjectKey, cancellationToken);
        await _fileAttachmentRepository.DeleteAsync(attachment, null, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
