using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Application.Apartments.Queries.GetApartmentById;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.Apartments.Queries.GetApartmentsByBuilding;

internal sealed class GetApartmentsByBuildingQueryHandler
    : IQueryHandler<GetApartmentsByBuildingQuery, IEnumerable<ApartmentResponse>>
{
    private readonly IApartmentRepository _apartmentRepository;

    public GetApartmentsByBuildingQueryHandler(IApartmentRepository apartmentRepository)
    {
        _apartmentRepository = apartmentRepository;
    }

    public async Task<Result<IEnumerable<ApartmentResponse>>> Handle(
        GetApartmentsByBuildingQuery request,
        CancellationToken cancellationToken)
    {
        var apartments = await _apartmentRepository.GetByBuildingIdAsync(
            request.BuildingId,
            cancellationToken);

        var response = apartments.Select(apartment => new ApartmentResponse(
            apartment.Id,
            apartment.BuildingId,
            apartment.Building.Name,
            apartment.ApartmentNumber,
            apartment.Floor,
            apartment.AreaSquareMeters,
            apartment.RoomCount,
            apartment.Status.ToString(),
            apartment.CurrentOwnerId,
            apartment.CurrentOwner?.FullName));

        return Result.Success(response);
    }
}
