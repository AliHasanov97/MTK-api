using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Buildings.Domain.Buildings.Events;

public sealed record BuildingUpdatedDomainEvent(Guid BuildingId, string BuildingName) : DomainEvent;
