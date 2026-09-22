using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Buildings.Domain.Garages.Events;

public sealed record GarageOwnerRemovedDomainEvent(
    Guid GarageId,
    Guid PreviousOwnerId) : DomainEvent;
