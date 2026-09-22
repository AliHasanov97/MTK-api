using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Buildings.Domain.Buildings.Events;

public sealed record BuildingCreatedDomainEvent(Guid BuildingId, string BuildingName) : DomainEvent;
