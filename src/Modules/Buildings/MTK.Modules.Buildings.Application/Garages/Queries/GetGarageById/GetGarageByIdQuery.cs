using MTK.Common.Application.Messaging;

namespace MTK.Modules.Buildings.Application.Garages.Queries.GetGarageById;

public sealed record GetGarageByIdQuery(Guid GarageId) : IQuery<GarageResponse>;
