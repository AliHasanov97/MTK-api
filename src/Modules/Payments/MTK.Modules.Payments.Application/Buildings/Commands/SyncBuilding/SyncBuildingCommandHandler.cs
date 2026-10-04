using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Buildings;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Buildings.Commands.SyncBuilding;

internal sealed class SyncBuildingCommandHandler : ICommandHandler<SyncBuildingCommand>
{
    private readonly IBuildingRepository _buildingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SyncBuildingCommandHandler(IBuildingRepository buildingRepository, IUnitOfWork unitOfWork)
    {
        _buildingRepository = buildingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SyncBuildingCommand request, CancellationToken cancellationToken)
    {
        var building = await _buildingRepository.GetByIdDefaultAsync(request.BuildingId, cancellationToken);
        if (building is not null)
        {
            building.Update(request.Name, request.Address);
        }
        else
        {
            building = Building.Create(request.BuildingId, request.Name, request.Address);
            _buildingRepository.Add(building);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
