using MTK.Common.Application.Messaging;
using MTK.Modules.Buildings.Application.Garages.Queries.GetGarageById;

namespace MTK.Modules.Buildings.Application.Garages.Queries.GetGaragesByOwnerId;

public sealed record GetGaragesByOwnerIdQuery(Guid OwnerId) : IQuery<IEnumerable<GarageResponse>>;
