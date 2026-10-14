namespace MTK.Modules.Buildings.Application.Buildings.Queries.GetBuildingById;

public sealed record BuildingResponse(
    Guid Id,
    string Name,
    string FullAddress,
    string Status,
    string? Description);
