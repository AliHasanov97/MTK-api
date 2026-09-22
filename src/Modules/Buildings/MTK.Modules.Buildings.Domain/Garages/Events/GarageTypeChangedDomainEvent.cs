using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Enums;

namespace MTK.Modules.Buildings.Domain.Garages.Events;

public sealed record GarageTypeChangedDomainEvent(
    Guid GarageId,
    GarageType OldType,
    GarageType NewType) : DomainEvent;
