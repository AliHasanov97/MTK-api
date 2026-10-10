using MTK.Common.Application.Storage;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.FileAttachments;

namespace MTK.Modules.Hr.Application.FileAttachments.DeleteFileAttachment;

public class DeleteFileAttachmentCommandHandler : ICommandHandler<DeleteFileAttachmentCommand>
{
    private readonly IFileAttachmentRepository _fileAttachmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public DeleteFileAttachmentCommandHandler(
        IUnitOfWork unitOfWork,
        IFileAttachmentRepository fileAttachmentRepository,
        IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _fileAttachmentRepository = fileAttachmentRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result> Handle(DeleteFileAttachmentCommand request, CancellationToken cancellationToken)
    {
        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var fileAttachment = await _fileAttachmentRepository.GetByIdDefaultAsync(request.Id, cancellationToken);
            if (fileAttachment == null)
            {
                return Result.Failure(FileAttachmentErrorMessages.NotFound);
            }

            await _fileStorageService.DeleteAsync(FileAttachmentStorageKey.For(fileAttachment.Id), cancellationToken);

            await _fileAttachmentRepository.DeleteAsync(fileAttachment, transaction, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(FileAttachmentErrorMessages.DeleteError);
        }
    }
}
