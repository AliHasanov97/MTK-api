namespace MTK.Modules.Buildings.Application.Buildings.Queries.GetBuildingById;

public sealed record BuildingResponse(
    Guid Id,
    string Name,
    string FullAddress,
    int TotalFloors,
    int ApartmentsPerFloor,
    int TotalApartments,
    string Status,
    string? Description);
