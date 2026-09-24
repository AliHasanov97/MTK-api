using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Enums;

namespace MTK.Modules.Buildings.Domain.Garages.Events;

public sealed record GarageCreatedDomainEvent(
    Guid GarageId,
    Guid? OwnerId,
    string GarageNumber,
    GarageType Type) : DomainEvent;
