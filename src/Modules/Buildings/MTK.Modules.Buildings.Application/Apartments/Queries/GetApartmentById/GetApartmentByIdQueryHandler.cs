using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
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

        var response = new ApartmentResponse(
            apartment.Id,
            apartment.BuildingId,
            apartment.Building.Name,
            apartment.ApartmentNumber,
            apartment.Floor,
            apartment.AreaSquareMeters,
            apartment.RoomCount,
            apartment.Status.ToString(),
            apartment.CurrentOwnerId,
            apartment.CurrentOwner?.FullName);

        return Result.Success(response);
    }
}
