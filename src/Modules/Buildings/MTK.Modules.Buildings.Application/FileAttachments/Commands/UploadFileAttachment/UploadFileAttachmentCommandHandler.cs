using MTK.Common.Application.Messaging;
using MTK.Common.Application.Storage;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Application.Abstractions.Data;
using MTK.Modules.Buildings.Domain.FileAttachments;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.FileAttachments.Commands.UploadFileAttachment;

internal sealed class UploadFileAttachmentCommandHandler : ICommandHandler<UploadFileAttachmentCommand, Guid>
{
    private readonly IFileAttachmentRepository _fileAttachmentRepository;
    private readonly IBuildingRepository _buildingRepository;
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IGarageRepository _garageRepository;
    private readonly IOwnerRepository _ownerRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;

    public UploadFileAttachmentCommandHandler(
        IFileAttachmentRepository fileAttachmentRepository,
        IBuildingRepository buildingRepository,
        IApartmentRepository apartmentRepository,
        IGarageRepository garageRepository,
        IOwnerRepository ownerRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork)
    {
        _fileAttachmentRepository = fileAttachmentRepository;
        _buildingRepository = buildingRepository;
        _apartmentRepository = apartmentRepository;
        _garageRepository = garageRepository;
        _ownerRepository = ownerRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(UploadFileAttachmentCommand request, CancellationToken cancellationToken)
    {
        if (!request.BuildingId.HasValue && !request.ApartmentId.HasValue
            && !request.GarageId.HasValue && !request.OwnerId.HasValue)
        {
            return Result.Failure<Guid>(new Error(
                "FileAttachment.NoTarget",
                "Ən azı bir əlaqəli obyekt (bina, mənzil, qaraj və ya sahib) göstərilməlidir"));
        }

        if (request.File.Length == 0)
        {
            return Result.Failure<Guid>(new Error("FileAttachment.EmptyFile", "Fayl boşdur"));
        }

        if (request.BuildingId.HasValue
            && await _buildingRepository.GetByIdDefaultAsync(request.BuildingId.Value, cancellationToken) is null)
        {
            return Result.Failure<Guid>(new Error("FileAttachment.BuildingNotFound", "Bina tapılmadı"));
        }

        if (request.ApartmentId.HasValue
            && await _apartmentRepository.GetByIdDefaultAsync(request.ApartmentId.Value, cancellationToken) is null)
        {
            return Result.Failure<Guid>(new Error("FileAttachment.ApartmentNotFound", "Mənzil tapılmadı"));
        }

        if (request.GarageId.HasValue
            && await _garageRepository.GetByIdDefaultAsync(request.GarageId.Value, cancellationToken) is null)
        {
            return Result.Failure<Guid>(new Error("FileAttachment.GarageNotFound", "Qaraj tapılmadı"));
        }

        if (request.OwnerId.HasValue
            && await _ownerRepository.GetByIdDefaultAsync(request.OwnerId.Value, cancellationToken) is null)
        {
            return Result.Failure<Guid>(new Error("FileAttachment.OwnerNotFound", "Sahib tapılmadı"));
        }

        var objectKey = $"buildings/{Guid.NewGuid()}-{request.File.FileName}";

        await using (var stream = request.File.OpenReadStream())
        {
            await _fileStorageService.UploadAsync(stream, objectKey, request.File.ContentType, cancellationToken);
        }

        var attachment = FileAttachment.Create(
            request.File.FileName,
            objectKey,
            request.File.ContentType,
            request.File.Length,
            request.BuildingId,
            request.ApartmentId,
            request.GarageId,
            request.OwnerId);

        _fileAttachmentRepository.Add(attachment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(attachment.Id);
    }
}
