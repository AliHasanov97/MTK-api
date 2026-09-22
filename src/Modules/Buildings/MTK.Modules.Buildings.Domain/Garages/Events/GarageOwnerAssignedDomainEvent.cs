using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Buildings.Domain.Garages.Events;

public sealed record GarageOwnerAssignedDomainEvent(
    Guid GarageId,
    Guid NewOwnerId,
    Guid? PreviousOwnerId) : DomainEvent;
