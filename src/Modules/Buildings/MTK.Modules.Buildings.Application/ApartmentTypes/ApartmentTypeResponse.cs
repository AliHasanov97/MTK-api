namespace MTK.Modules.Buildings.Application.ApartmentTypes;

public sealed record ApartmentTypeResponse(
    Guid Id,
    Guid BuildingId,
    int RoomCount,
    decimal AreaSquareMeters);
