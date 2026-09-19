using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Identity.Domain.Groups.Events;

public sealed record UserGroupAssignedDomainEvent(
    Guid UserId,
    Guid GroupId,
    DateTime OccurredOnUtc) : IDomainEvent;
