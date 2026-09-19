using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Identity.Domain.Roles.Events;

public sealed record UserRoleRemovedDomainEvent(
    Guid UserId,
    List<Guid> RoleIds,
    DateTime OccurredOnUtc) : IDomainEvent;
