using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Nomenclatures.Commands.DeactivateNomenclature;

/// <summary>
/// Warehouse-da nomenklatura silindikdə (soft delete) güzgünü deaktiv edir — sətir
/// silinmir, çünki keçmiş alış sətirləri ona istinad edir.
/// </summary>
public sealed record DeactivateNomenclatureCommand(Guid NomenclatureId) : ICommand;
