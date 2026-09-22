using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Buildings.Domain.Apartments.Events;

public sealed record ApartmentOwnerAssignedDomainEvent(
    Guid ApartmentId,
    Guid NewOwnerId,
    Guid? PreviousOwnerId) : DomainEvent;
