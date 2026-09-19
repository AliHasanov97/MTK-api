using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Identity.Domain.Users.Events;

public sealed record UserDeletedDomainEvent(
    Guid UserId,
    DateTime OccurredOnUtc) : IDomainEvent;
