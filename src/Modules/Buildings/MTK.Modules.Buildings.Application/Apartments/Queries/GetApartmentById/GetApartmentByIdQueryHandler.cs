using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Common.Presentation.Responses;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.Apartments.Queries.GetApartmentById;

internal sealed class GetApartmentByIdQueryHandler
    : IQueryHandler<GetApartmentByIdQuery, ApartmentResponse>
{
    private readonly IApartmentRepository _apartmentRepository;

    public GetApartmentByIdQueryHandler(IApartmentRepository apartmentRepository)
    {
        _apartmentRepository = apartmentRepository;
    }

    public async Task<Result<ApartmentResponse>> Handle(
        GetApartmentByIdQuery request,
        CancellationToken cancellationToken)
    {
        var apartment = await _apartmentRepository.GetByIdAsync(
            request.ApartmentId,
            cancellationToken);

        if (apartment is null)
        {
            return Result.Failure<ApartmentResponse>(new Error(
                "Apartment.NotFound",
                $"Mənzil tapılmadı: {request.ApartmentId}"));
        }

        var building = ResponseObjectWithName.Create(apartment.BuildingId, apartment.Building.Name);
        var currentOwner = apartment.CurrentOwnerId.HasValue && apartment.CurrentOwner != null
            ? ResponseObjectWithName.Create(apartment.CurrentOwnerId.Value, apartment.CurrentOwner.FullName)
            : null;

        var response = new ApartmentResponse(
            apartment.Id,
            building,
            apartment.ApartmentNumber,
            apartment.Floor,
            apartment.AreaSquareMeters,
            apartment.RoomCount,
            apartment.Status.ToString(),
            currentOwner);

        return Result.Success(response);
    }
}
