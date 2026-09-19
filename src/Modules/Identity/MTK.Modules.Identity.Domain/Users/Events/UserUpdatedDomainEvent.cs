using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Identity.Domain.Users.Events;

public sealed record UserUpdatedDomainEvent(
    Guid UserId,
    DateTime OccurredOnUtc) : IDomainEvent;
