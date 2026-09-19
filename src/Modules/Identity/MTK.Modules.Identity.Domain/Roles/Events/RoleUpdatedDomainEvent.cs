using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Identity.Domain.Roles.Events;

public sealed record RoleUpdatedDomainEvent(
    Guid RoleId,
    string Name,
    string? Description,
    RoleType RoleType,
    bool IsActive,
    DateTime OccurredOnUtc) : IDomainEvent;
