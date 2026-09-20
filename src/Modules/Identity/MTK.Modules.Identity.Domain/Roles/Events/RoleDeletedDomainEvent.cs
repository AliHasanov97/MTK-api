using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Identity.Domain.Roles.Events;

public sealed record RoleDeletedDomainEvent(
    Guid RoleId,
    string Name,
    DateTime OccurredOnUtc) : DomainEvent;
