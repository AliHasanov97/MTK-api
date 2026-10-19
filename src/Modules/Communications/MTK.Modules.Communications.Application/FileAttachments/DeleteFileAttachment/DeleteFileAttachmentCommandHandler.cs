using MTK.Common.Application.Messaging;
using MTK.Common.Application.Storage;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Communications.Application.Abstractions.Data;
using MTK.Modules.Communications.Domain.FileAttachments;

namespace MTK.Modules.Communications.Application.FileAttachments.DeleteFileAttachment;

internal sealed class DeleteFileAttachmentCommandHandler : ICommandHandler<DeleteFileAttachmentCommand>
{
    private readonly IFileAttachmentRepository _fileAttachmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public DeleteFileAttachmentCommandHandler(
        IFileAttachmentRepository fileAttachmentRepository,
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorageService)
    {
        _fileAttachmentRepository = fileAttachmentRepository;
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result> Handle(DeleteFileAttachmentCommand request, CancellationToken cancellationToken)
    {
        var attachment = await _fileAttachmentRepository.GetByIdDefaultAsync(request.Id, cancellationToken);
        if (attachment is null)
        {
            return Result.Failure(FileAttachmentErrors.NotFound);
        }

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await _fileStorageService.DeleteAsync(FileAttachmentStorageKey.For(attachment.Id), cancellationToken);

            await _fileAttachmentRepository.DeleteAsync(attachment, transaction, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(FileAttachmentErrors.DeleteFailed);
        }
    }
}
