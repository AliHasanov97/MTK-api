using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Enums;

namespace MTK.Modules.Buildings.Domain.Apartments.Events;

public sealed record ApartmentStatusChangedDomainEvent(
    Guid ApartmentId,
    ApartmentStatus OldStatus,
    ApartmentStatus NewStatus) : DomainEvent;
