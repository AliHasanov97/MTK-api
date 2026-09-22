using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Buildings.Domain.Owners.Events;

public sealed record OwnerCreatedDomainEvent(
    Guid OwnerId,
    Guid UserId,
    string FullName) : DomainEvent;
