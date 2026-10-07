using MTK.Common.Application.EventBus;

namespace MTK.Modules.Warehouse.IntegrationEvents.Nomenclatures;

/// <summary>
/// Nomenklatura dəyişdikdə digər modulların saxladığı güzgü məlumatını yeniləmək üçün.
/// </summary>
public sealed class NomenclatureUpdatedIntegrationEvent : IntegrationEvent
{
    public NomenclatureUpdatedIntegrationEvent(
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
