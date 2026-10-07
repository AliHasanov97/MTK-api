using MTK.Common.Application.EventBus;

namespace MTK.Modules.Warehouse.IntegrationEvents.Nomenclatures;

/// <summary>
/// Yeni nomenklatura yaradıldıqda digər module-lara bildiriş — başqa modullar
/// (məs. uçot/xərc) öz nomenklatura güzgüsünü bununla sinxronlaşdıra bilər.
/// </summary>
public sealed class NomenclatureCreatedIntegrationEvent : IntegrationEvent
{
    public NomenclatureCreatedIntegrationEvent(
        Guid integrationEventId,
        DateTime occurredOnUtc,
        Guid nomenclatureId,
        string code,
        string name,
        string? description,
        int category,
        int unit,
        decimal? minStockLevel,
        bool isActive)
        : base(integrationEventId, occurredOnUtc)
    {
        NomenclatureId = nomenclatureId;
        Code = code;
        Name = name;
        Description = description;
        Category = category;
        Unit = unit;
        MinStockLevel = minStockLevel;
        IsActive = isActive;
    }

    public Guid NomenclatureId { get; }
    public string Code { get; }
    public string Name { get; }
    public string? Description { get; }
    public int Category { get; }
    public int Unit { get; }
    public decimal? MinStockLevel { get; }
    public bool IsActive { get; }
}
