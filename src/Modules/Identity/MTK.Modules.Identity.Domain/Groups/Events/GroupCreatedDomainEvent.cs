using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Identity.Domain.Groups.Events;

public sealed record GroupCreatedDomainEvent(
    Guid GroupId,
    Guid KeycloakGroupId,
    string Name,
    string? Description,
    Guid? ParentGroupId,
    DateTime OccurredOnUtc) : IDomainEvent;
