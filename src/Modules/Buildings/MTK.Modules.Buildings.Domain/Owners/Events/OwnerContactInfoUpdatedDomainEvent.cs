using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Buildings.Domain.Owners.Events;

public sealed record OwnerContactInfoUpdatedDomainEvent(
    Guid OwnerId,
    string FullName,
    string Email,
    string PhoneNumber) : DomainEvent;
