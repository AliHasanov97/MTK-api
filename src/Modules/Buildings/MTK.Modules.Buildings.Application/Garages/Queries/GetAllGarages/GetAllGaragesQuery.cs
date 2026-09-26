using MTK.Common.Application.Messaging;
using MTK.Modules.Buildings.Application.Garages.Queries.GetGarageById;

namespace MTK.Modules.Buildings.Application.Garages.Queries.GetAllGarages;

public sealed record GetAllGaragesQuery() : IQuery<IEnumerable<GarageResponse>>;
