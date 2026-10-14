using MTK.Common.Application.Messaging;

namespace MTK.Modules.Buildings.Application.ApartmentTypes.Queries.GetApartmentTypesByBuilding;

public sealed record GetApartmentTypesByBuildingQuery(Guid BuildingId)
    : IQuery<IEnumerable<ApartmentTypeResponse>>;
