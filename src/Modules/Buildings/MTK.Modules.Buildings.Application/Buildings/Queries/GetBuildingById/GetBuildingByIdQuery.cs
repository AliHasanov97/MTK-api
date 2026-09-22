using MTK.Common.Application.Messaging;

namespace MTK.Modules.Buildings.Application.Buildings.Queries.GetBuildingById;

public sealed record GetBuildingByIdQuery(Guid BuildingId) : IQuery<BuildingResponse>;
