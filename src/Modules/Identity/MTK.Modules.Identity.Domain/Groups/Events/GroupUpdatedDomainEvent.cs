using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Identity.Domain.Groups.Events;

public sealed record GroupUpdatedDomainEvent(
    Guid GroupId,
    Guid KeycloakGroupId,
    string Name,
    string? Description,
    DateTime OccurredOnUtc) : DomainEvent;
