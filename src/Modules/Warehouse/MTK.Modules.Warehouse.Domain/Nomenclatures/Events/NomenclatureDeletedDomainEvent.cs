using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Warehouse.Domain.Nomenclatures.Events;

public sealed record NomenclatureDeletedDomainEvent(Guid NomenclatureId) : DomainEvent;
