using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.Buildings.Queries.GetBuildingById;

internal sealed class GetBuildingByIdQueryHandler
    : IQueryHandler<GetBuildingByIdQuery, BuildingResponse>
{
    private readonly IBuildingRepository _buildingRepository;
    private readonly IMapper _mapper;

    public GetBuildingByIdQueryHandler(IBuildingRepository buildingRepository, IMapper mapper)
    {
        _buildingRepository = buildingRepository;
        _mapper = mapper;
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

        return Result.Success(_mapper.Map<BuildingResponse>(building));
    }
}
