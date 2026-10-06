using MTK.Common.Domain.Abstractions;
using MTK.Modules.Warehouse.Domain.Nomenclatures;

namespace MTK.Modules.Warehouse.Application.Nomenclatures.GetAllNomenclatures;

public sealed record GetAllNomenclaturesQuery(
    int PageNumber = 1,
    int PageSize = 50,
    bool? IsActive = null) : IQuery<Result<List<NomenclatureDto>>>;

public sealed record NomenclatureDto(
    Guid Id,
    string Code,
    string Name,
    NomenclatureCategory Category,
    Unit Unit,
    bool IsActive);
