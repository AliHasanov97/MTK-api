using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Buildings.Domain.Apartments.Events;

public sealed record ApartmentUpdatedDomainEvent(Guid ApartmentId, Guid BuildingId) : DomainEvent;
