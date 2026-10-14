using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Application.Abstractions.Data;
using MTK.Modules.Buildings.Domain.ApartmentTypes;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.ApartmentTypes.Commands.CreateApartmentType;

internal sealed class CreateApartmentTypeCommandHandler
    : ICommandHandler<CreateApartmentTypeCommand, Guid>
{
    private readonly IApartmentTypeRepository _apartmentTypeRepository;
    private readonly IBuildingRepository _buildingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateApartmentTypeCommandHandler(
        IApartmentTypeRepository apartmentTypeRepository,
        IBuildingRepository buildingRepository,
        IUnitOfWork unitOfWork)
    {
        _apartmentTypeRepository = apartmentTypeRepository;
        _buildingRepository = buildingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CreateApartmentTypeCommand request,
        CancellationToken cancellationToken)
    {
        var building = await _buildingRepository.GetByIdAsync(
            request.BuildingId,
            cancellationToken);

        if (building is null)
        {
            return Result.Failure<Guid>(new Error(
                "Building.NotFound",
                $"Bina tapılmadı: {request.BuildingId}"));
        }

        var apartmentType = ApartmentType.Create(
            request.BuildingId,
            request.RoomCount,
            request.AreaSquareMeters);

        _apartmentTypeRepository.Add(apartmentType);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(apartmentType.Id);
    }
}
