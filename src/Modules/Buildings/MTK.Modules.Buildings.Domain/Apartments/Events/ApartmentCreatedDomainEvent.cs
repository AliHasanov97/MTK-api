using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Buildings.Domain.Apartments.Events;

public sealed record ApartmentCreatedDomainEvent(
    Guid ApartmentId,
    Guid BuildingId,
    string ApartmentNumber) : DomainEvent;
