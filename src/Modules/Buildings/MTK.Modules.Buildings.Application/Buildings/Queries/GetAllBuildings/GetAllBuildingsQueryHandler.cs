using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Application.Buildings.Queries.GetBuildingById;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.Buildings.Queries.GetAllBuildings;

internal sealed class GetAllBuildingsQueryHandler
    : IQueryHandler<GetAllBuildingsQuery, IEnumerable<BuildingResponse>>
{
    private readonly IBuildingRepository _buildingRepository;
    private readonly IMapper _mapper;

    public GetAllBuildingsQueryHandler(IBuildingRepository buildingRepository, IMapper mapper)
    {
        _buildingRepository = buildingRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<BuildingResponse>>> Handle(
        GetAllBuildingsQuery request,
        CancellationToken cancellationToken)
    {
        var buildings = await _buildingRepository.GetAllAsync(cancellationToken);

        return Result.Success(_mapper.Map<IEnumerable<BuildingResponse>>(buildings));
    }
}
