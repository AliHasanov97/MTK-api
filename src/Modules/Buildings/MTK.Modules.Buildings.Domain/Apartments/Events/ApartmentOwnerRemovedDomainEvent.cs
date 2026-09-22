using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Buildings.Domain.Apartments.Events;

public sealed record ApartmentOwnerRemovedDomainEvent(
    Guid ApartmentId,
    Guid PreviousOwnerId) : DomainEvent;
