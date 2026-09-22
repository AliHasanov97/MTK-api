using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Enums;

namespace MTK.Modules.Buildings.Domain.Buildings.Events;

public sealed record BuildingStatusChangedDomainEvent(
    Guid BuildingId,
    BuildingStatus OldStatus,
    BuildingStatus NewStatus) : DomainEvent;
