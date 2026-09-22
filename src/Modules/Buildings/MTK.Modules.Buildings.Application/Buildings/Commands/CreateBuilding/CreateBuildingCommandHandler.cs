using MTK.Common.Domain.Abstractions;
using MTK.Common.Application.Messaging;
using MTK.Modules.Buildings.Application.Abstractions.Data;
using MTK.Modules.Buildings.Domain.Buildings;
using MTK.Modules.Buildings.Domain.Repositories;
using MTK.Modules.Buildings.Domain.ValueObjects;

namespace MTK.Modules.Buildings.Application.Buildings.Commands.CreateBuilding;

internal sealed class CreateBuildingCommandHandler
    : ICommandHandler<CreateBuildingCommand, Guid>
{
    private readonly IBuildingRepository _buildingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBuildingCommandHandler(
        IBuildingRepository buildingRepository,
        IUnitOfWork unitOfWork)
    {
        _buildingRepository = buildingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CreateBuildingCommand request,
        CancellationToken cancellationToken)
    {
        var exists = await _buildingRepository.ExistsByNameAsync(
            request.Name,
            cancellationToken);

        if (exists)
        {
            return Result.Failure<Guid>(new Error(
                "Building.AlreadyExists",
                $"Bu adlı bina artıq mövcuddur: {request.Name}"));
        }

        var address = new Address(
            request.Street,
            request.City,
            request.District);

        var building = Building.Create(
            request.Name,
            address,
            request.TotalFloors,
            request.ApartmentsPerFloor,
            request.Description);

        _buildingRepository.Add(building);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(building.Id);
    }
}
