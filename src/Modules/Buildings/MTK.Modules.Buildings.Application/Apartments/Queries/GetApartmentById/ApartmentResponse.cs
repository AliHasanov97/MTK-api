namespace MTK.Modules.Buildings.Application.Apartments.Queries.GetApartmentById;

public sealed record ApartmentResponse(
    Guid Id,
    Guid BuildingId,
    string BuildingName,
    string ApartmentNumber,
    int Floor,
    decimal AreaSquareMeters,
    int RoomCount,
    string Status,
    Guid? CurrentOwnerId,
    string? CurrentOwnerName);
