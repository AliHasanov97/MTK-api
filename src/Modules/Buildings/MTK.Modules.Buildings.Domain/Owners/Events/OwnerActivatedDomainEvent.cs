using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Buildings.Domain.Owners.Events;

public sealed record OwnerActivatedDomainEvent(Guid OwnerId) : DomainEvent;
