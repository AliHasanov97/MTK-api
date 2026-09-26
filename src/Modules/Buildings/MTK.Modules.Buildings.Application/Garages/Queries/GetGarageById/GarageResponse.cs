using MTK.Modules.Buildings.Application.Garages.Queries.Common;

namespace MTK.Modules.Buildings.Application.Garages.Queries.GetGarageById;

public sealed record GarageResponse(
    Guid Id,
    string GarageNumber,
    string Type,
    string? Description,
    OwnerInfo? Owner,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);