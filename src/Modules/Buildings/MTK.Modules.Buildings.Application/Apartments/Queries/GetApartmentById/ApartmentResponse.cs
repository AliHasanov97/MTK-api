using MTK.Common.Presentation.Responses;

namespace MTK.Modules.Buildings.Application.Apartments.Queries.GetApartmentById;

public sealed record ApartmentResponse(
    Guid Id,
    ResponseObjectWithName Building,
    string ApartmentNumber,
    int Floor,
    decimal AreaSquareMeters,
    int RoomCount,
    string Status,
    ResponseObjectWithName? CurrentOwner);
