using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Identity.Domain.Roles.Events;

public sealed record RoleCreatedDomainEvent(
    Guid RoleId,
    string Name,
    string? Description,
    RoleType RoleType,
    DateTime OccurredOnUtc) : DomainEvent;
