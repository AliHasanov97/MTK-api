using MTK.Common.Application.Messaging;
using MTK.Modules.Warehouse.Domain.Nomenclatures;

namespace MTK.Modules.Warehouse.Application.Nomenclatures.Commands.UpdateNomenclature;

public sealed record UpdateNomenclatureCommand(
    Guid Id,
    string Name,
    string? Description,
    NomenclatureCategory Category,
    Unit Unit,
    decimal? MinStockLevel) : ICommand;
