using MTK.Common.Domain.Abstractions;
using MTK.Modules.Warehouse.Domain.Nomenclatures;

namespace MTK.Modules.Warehouse.Application.Nomenclatures.GetNomenclatureById;

public sealed record GetNomenclatureByIdQuery(Guid Id) : IQuery<Result<NomenclatureResponse>>;

public sealed record NomenclatureResponse(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    NomenclatureCategory Category,
    Unit Unit,
    decimal? MinStockLevel,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
