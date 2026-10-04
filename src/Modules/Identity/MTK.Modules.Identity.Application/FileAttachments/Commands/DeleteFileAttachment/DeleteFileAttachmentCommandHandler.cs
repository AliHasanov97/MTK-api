using MTK.Common.Application.Messaging;
using MTK.Common.Application.Storage;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions.Data;
using MTK.Modules.Identity.Domain.FileAttachments;

namespace MTK.Modules.Identity.Application.FileAttachments.Commands.DeleteFileAttachment;

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

        await _fileStorageService.DeleteAsync(attachment.ObjectKey, cancellationToken);
        await _fileAttachmentRepository.DeleteAsync(attachment, null, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
