using MTK.Common.Application.Messaging;
using MTK.Modules.Buildings.Application.Apartments.Queries.GetApartmentById;

namespace MTK.Modules.Buildings.Application.Apartments.Queries.GetApartmentsByBuilding;

public sealed record GetApartmentsByBuildingQuery(Guid BuildingId)
    : IQuery<IEnumerable<ApartmentResponse>>;
