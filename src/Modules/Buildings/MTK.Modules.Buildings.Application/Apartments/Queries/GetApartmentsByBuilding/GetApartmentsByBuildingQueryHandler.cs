using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Common.Presentation.Responses;
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

        var response = apartments.Select(apartment =>
        {
            var building = ResponseObjectWithName.Create(apartment.BuildingId, apartment.Building.Name);
            var currentOwner = apartment.CurrentOwnerId.HasValue && apartment.CurrentOwner != null
                ? ResponseObjectWithName.Create(apartment.CurrentOwnerId.Value, apartment.CurrentOwner.FullName)
                : null;

            return new ApartmentResponse(
                apartment.Id,
                building,
                apartment.ApartmentNumber,
                apartment.Floor,
                apartment.AreaSquareMeters,
                apartment.RoomCount,
                apartment.Status.ToString(),
                currentOwner);
        });

        return Result.Success(response);
    }
}
