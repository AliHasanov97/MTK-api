using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Identity.Domain.Groups.Events;

public sealed record GroupRoleAssignedDomainEvent(
    Guid GroupId,
    List<Guid> RoleIds,
    DateTime OccurredOnUtc) : IDomainEvent;
