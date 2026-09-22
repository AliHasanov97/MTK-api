using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Application.Buildings.Queries.GetBuildingById;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.Buildings.Queries.GetAllBuildings;

internal sealed class GetAllBuildingsQueryHandler
    : IQueryHandler<GetAllBuildingsQuery, IEnumerable<BuildingResponse>>
{
    private readonly IBuildingRepository _buildingRepository;

    public GetAllBuildingsQueryHandler(IBuildingRepository buildingRepository)
    {
        _buildingRepository = buildingRepository;
    }

    public async Task<Result<IEnumerable<BuildingResponse>>> Handle(
        GetAllBuildingsQuery request,
        CancellationToken cancellationToken)
    {
        var buildings = await _buildingRepository.GetAllAsync(cancellationToken);

        var response = buildings.Select(building => new BuildingResponse(
            building.Id,
            building.Name,
            building.Address.GetFullAddress(),
            building.TotalFloors,
            building.ApartmentsPerFloor,
            building.TotalApartments,
            building.Status.ToString(),
            building.Description));

        return Result.Success(response);
    }
}
