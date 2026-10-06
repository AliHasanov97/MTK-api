using MTK.Common.Domain.Abstractions;
using MTK.Modules.Warehouse.Domain.Nomenclatures;

namespace MTK.Modules.Warehouse.Application.Nomenclatures.SearchNomenclatures;

public sealed record SearchNomenclaturesQuery(
    string? SearchTerm = null,
    NomenclatureCategory? Category = null,
    bool? IsActive = null,
    int PageNumber = 1,
    int PageSize = 50) : IQuery<Result<List<NomenclatureSearchDto>>>;

public sealed record NomenclatureSearchDto(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    NomenclatureCategory Category,
    Unit Unit,
    bool IsActive);
