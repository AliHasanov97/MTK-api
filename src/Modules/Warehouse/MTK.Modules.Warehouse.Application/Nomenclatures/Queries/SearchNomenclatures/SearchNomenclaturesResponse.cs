using MTK.Modules.Warehouse.Domain.Nomenclatures;

namespace MTK.Modules.Warehouse.Application.Nomenclatures.Queries.SearchNomenclatures;

public sealed record SearchNomenclaturesResponse(
    IReadOnlyCollection<NomenclatureListItem> Nomenclatures,
    int TotalCount,
    int Page,
    int PageSize);

public sealed record NomenclatureListItem(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    NomenclatureCategory Category,
    Unit Unit,
    bool IsActive);
