using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Buildings.Domain.Garages.Events;

public sealed record GarageDeletedDomainEvent(Guid GarageId) : DomainEvent;
