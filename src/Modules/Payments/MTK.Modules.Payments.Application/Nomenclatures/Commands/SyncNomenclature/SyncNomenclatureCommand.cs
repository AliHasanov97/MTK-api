using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Nomenclatures.Commands.SyncNomenclature;

/// <summary>
/// Warehouse-dakı nomenklaturanın bu moduldakı güzgüsünü yaradır/yeniləyir (upsert).
/// Idempotentdir — integration event təkrar çatdırıla bilər (at-least-once).
/// </summary>
public sealed record SyncNomenclatureCommand(
    Guid NomenclatureId,
    string Code,
    string Name,
    string? Description,
    int Category,
    int Unit,
    decimal? MinStockLevel,
    bool IsActive) : ICommand;
