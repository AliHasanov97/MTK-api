using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Buildings.Domain.Owners.Events;

/// <summary>
/// Passive owner User account ilə link ediləndə raise olunur
/// </summary>
public sealed record OwnerLinkedToUserDomainEvent(
    Guid OwnerId,
    Guid UserId) : DomainEvent;
