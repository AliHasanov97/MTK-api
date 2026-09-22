using MTK.Common.Application.Messaging;

namespace MTK.Modules.Buildings.Application.Apartments.Queries.GetApartmentById;

public sealed record GetApartmentByIdQuery(Guid ApartmentId) : IQuery<ApartmentResponse>;
