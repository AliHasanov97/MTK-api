using MTK.Common.Application.Messaging;
using MTK.Modules.Warehouse.Domain.Nomenclatures;

namespace MTK.Modules.Warehouse.Application.Nomenclatures.Commands.CreateNomenclature;

public sealed record CreateNomenclatureCommand(
    string Code,
    string Name,
    string? Description,
    NomenclatureCategory Category,
    Unit Unit,
    decimal? MinStockLevel,
    bool IsActive = true) : ICommand<Guid>;
