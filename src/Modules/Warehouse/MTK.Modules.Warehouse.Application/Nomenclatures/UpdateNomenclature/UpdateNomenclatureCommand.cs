using MTK.Common.Domain.Abstractions;
using MTK.Modules.Warehouse.Domain.Nomenclatures;

namespace MTK.Modules.Warehouse.Application.Nomenclatures.UpdateNomenclature;

public sealed record UpdateNomenclatureCommand(
    Guid Id,
    string Name,
    string? Description,
    NomenclatureCategory Category,
    Unit Unit,
    decimal? MinStockLevel) : ICommand<Result>;
