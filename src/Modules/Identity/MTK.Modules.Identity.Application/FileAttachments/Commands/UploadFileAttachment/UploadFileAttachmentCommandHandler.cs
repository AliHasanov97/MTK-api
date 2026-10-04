using MTK.Common.Application.Messaging;
using MTK.Common.Application.Storage;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions.Data;
using MTK.Modules.Identity.Domain.FileAttachments;
using MTK.Modules.Identity.Domain.Users;

namespace MTK.Modules.Identity.Application.FileAttachments.Commands.UploadFileAttachment;

internal sealed class UploadFileAttachmentCommandHandler : ICommandHandler<UploadFileAttachmentCommand, Guid>
{
    private readonly IFileAttachmentRepository _fileAttachmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;

    public UploadFileAttachmentCommandHandler(
        IFileAttachmentRepository fileAttachmentRepository,
        IUserRepository userRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork)
    {
        _fileAttachmentRepository = fileAttachmentRepository;
        _userRepository = userRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(UploadFileAttachmentCommand request, CancellationToken cancellationToken)
    {
        if (request.File.Length == 0)
        {
            return Result.Failure<Guid>(new Error("FileAttachment.EmptyFile", "Fayl boşdur"));
        }

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<Guid>(new Error("FileAttachment.UserNotFound", $"İstifadəçi tapılmadı: {request.UserId}"));
        }

        var objectKey = $"identity/users/{request.UserId}/{Guid.NewGuid()}-{request.File.FileName}";

        await using (var stream = request.File.OpenReadStream())
        {
            await _fileStorageService.UploadAsync(stream, objectKey, request.File.ContentType, cancellationToken);
        }

        var attachment = FileAttachment.Create(
            request.File.FileName,
            objectKey,
            request.File.ContentType,
            request.File.Length,
            request.UserId);

        _fileAttachmentRepository.Add(attachment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(attachment.Id);
    }
}
