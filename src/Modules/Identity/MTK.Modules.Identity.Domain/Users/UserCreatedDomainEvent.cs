using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Identity.Domain.Users;

public sealed record UserCreatedDomainEvent(
    Guid UserId,
    string[] RoleNames) : DomainEvent;
