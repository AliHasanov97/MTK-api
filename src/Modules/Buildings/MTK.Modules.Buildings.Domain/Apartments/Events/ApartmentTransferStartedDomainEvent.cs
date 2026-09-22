using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Buildings.Domain.Apartments.Events;

public sealed record ApartmentTransferStartedDomainEvent(
    Guid ApartmentId,
    Guid? CurrentOwnerId) : DomainEvent;
