using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Buildings.Domain.Owners.Events;

public sealed record OwnerDeactivatedDomainEvent(Guid OwnerId) : DomainEvent;
