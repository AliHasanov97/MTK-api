using MTK.Common.Application.Messaging;
using MTK.Modules.Buildings.Application.Buildings.Queries.GetBuildingById;

namespace MTK.Modules.Buildings.Application.Buildings.Queries.GetAllBuildings;

public sealed record GetAllBuildingsQuery() : IQuery<IEnumerable<BuildingResponse>>;
