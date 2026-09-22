using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.Buildings.Queries.GetBuildingById;

internal sealed class GetBuildingByIdQueryHandler
    : IQueryHandler<GetBuildingByIdQuery, BuildingResponse>
{
    private readonly IBuildingRepository _buildingRepository;

    public GetBuildingByIdQueryHandler(IBuildingRepository buildingRepository)
    {
        _buildingRepository = buildingRepository;
    }

    public async Task<Result<BuildingResponse>> Handle(
        GetBuildingByIdQuery request,
        CancellationToken cancellationToken)
    {
        var building = await _buildingRepository.GetByIdAsync(
            request.BuildingId,
            cancellationToken);

        if (building is null)
        {
            return Result.Failure<BuildingResponse>(new Error(
                "Building.NotFound",
                $"Bina tapılmadı: {request.BuildingId}"));
        }

        var response = new BuildingResponse(
            building.Id,
            building.Name,
            building.Address.GetFullAddress(),
            building.TotalFloors,
            building.ApartmentsPerFloor,
            building.TotalApartments,
            building.Status.ToString(),
            building.Description);

        return Result.Success(response);
    }
}
