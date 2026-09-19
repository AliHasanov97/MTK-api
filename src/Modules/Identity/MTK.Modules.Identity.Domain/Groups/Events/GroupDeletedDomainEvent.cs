using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Identity.Domain.Groups.Events;

public sealed record GroupDeletedDomainEvent(
    Guid GroupId,
    Guid KeycloakGroupId,
    string Name,
    DateTime OccurredOnUtc) : IDomainEvent;
