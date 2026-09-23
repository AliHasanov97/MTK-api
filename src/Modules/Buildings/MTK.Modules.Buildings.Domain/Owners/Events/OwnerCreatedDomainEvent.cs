using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Buildings.Domain.Owners.Events;

public sealed record OwnerCreatedDomainEvent(
    Guid OwnerId,
    Guid? UserId,  // Nullable - passive owners don't have user
    string FullName) : DomainEvent;
