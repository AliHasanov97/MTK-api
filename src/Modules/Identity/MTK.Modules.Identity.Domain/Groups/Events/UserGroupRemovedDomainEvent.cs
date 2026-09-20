using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Identity.Domain.Groups.Events;

public sealed record UserGroupRemovedDomainEvent(
    Guid UserId,
    Guid GroupId,
    DateTime OccurredOnUtc) : DomainEvent;
