using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.ApartmentTypes.Queries.GetApartmentTypesByBuilding;

internal sealed class GetApartmentTypesByBuildingQueryHandler
    : IQueryHandler<GetApartmentTypesByBuildingQuery, IEnumerable<ApartmentTypeResponse>>
{
    private readonly IApartmentTypeRepository _apartmentTypeRepository;

    public GetApartmentTypesByBuildingQueryHandler(IApartmentTypeRepository apartmentTypeRepository)
    {
        _apartmentTypeRepository = apartmentTypeRepository;
    }

    public async Task<Result<IEnumerable<ApartmentTypeResponse>>> Handle(
        GetApartmentTypesByBuildingQuery request,
        CancellationToken cancellationToken)
    {
        var types = await _apartmentTypeRepository.GetByBuildingIdAsync(
            request.BuildingId,
            cancellationToken);

        IEnumerable<ApartmentTypeResponse> response = types
            .Select(t => new ApartmentTypeResponse(t.Id, t.BuildingId, t.RoomCount, t.AreaSquareMeters))
            .ToList();

        return Result.Success(response);
    }
}
