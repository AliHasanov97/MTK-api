using MTK.Common.Application.Messaging;
using MTK.Modules.Warehouse.Domain.Nomenclatures;

namespace MTK.Modules.Warehouse.Application.Nomenclatures.Queries.GetNomenclatureById;

public sealed record GetNomenclatureByIdQuery(Guid Id) : IQuery<NomenclatureResponse>;

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
