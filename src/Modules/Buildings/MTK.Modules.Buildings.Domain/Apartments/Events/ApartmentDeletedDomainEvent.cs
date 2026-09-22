using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Buildings.Domain.Apartments.Events;

public sealed record ApartmentDeletedDomainEvent(Guid ApartmentId) : DomainEvent;
