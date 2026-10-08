using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.Nomenclatures;

/// <summary>
/// Warehouse modulundakı Nomenclature-in bu moduldakı güzgü (shadow) kopyası —
/// Id Warehouse-dakı Nomenclature.Id ilə eynidir. Nomenklatura yalnız Warehouse-da
/// yaradılır/dəyişdirilir; burada <c>Nomenclature{Created,Updated,Deleted}IntegrationEvent</c>
/// ilə sinxronlaşdırılır ki, alış sətirləri (PurchaseLine) nomenklaturaya istinad edə bilsin.
///
/// Category/Unit Warehouse enum-unun rəqəmsal ordinalları kimi saxlanılır (modul sərhədini
/// keçən müqavilə int-dir) — təkrar enum tərifini burada saxlamamaq üçün.
/// </summary>
public sealed class NomenclatureShadow : SearchableEntity
{
    private NomenclatureShadow() : base() { }

    /// <summary>Warehouse-dakı unikal nomenklatura kodu.</summary>
    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    /// <summary>Warehouse NomenclatureCategory ordinal-ı.</summary>
    public int Category { get; private set; }

    /// <summary>Warehouse Unit ordinal-ı.</summary>
    public int Unit { get; private set; }

    public decimal? MinStockLevel { get; private set; }

    /// <summary>Warehouse-da silinmiş nomenklatura burada deaktiv edilir (sətir qalır — tarixçə üçün).</summary>
    public bool IsActive { get; private set; }

    /// <summary>Son sync tarixi.</summary>
    public DateTimeOffset SyncedAtUtc { get; private set; }

    public static NomenclatureShadow Create(
        Guid id,
        string code,
        string name,
        string? description,
        int category,
        int unit,
        decimal? minStockLevel,
        bool isActive)
    {
        var nomenclature = new NomenclatureShadow
        {
            Id = id,
            Code = code,
            Name = name,
            Description = description,
            Category = category,
            Unit = unit,
            MinStockLevel = minStockLevel,
            IsActive = isActive,
            SyncedAtUtc = DateTimeOffset.UtcNow
        };

        nomenclature.SetCreatedAt();
        return nomenclature;
    }

    /// <summary>Warehouse-dan gələn yeniləməni tətbiq edir (idempotent upsert).</summary>
    public void Sync(
        string code,
        string name,
        string? description,
        int category,
        int unit,
        decimal? minStockLevel,
        bool isActive)
    {
        Code = code;
        Name = name;
        Description = description;
        Category = category;
        Unit = unit;
        MinStockLevel = minStockLevel;
        IsActive = isActive;
        SyncedAtUtc = DateTimeOffset.UtcNow;
        SetUpdatedAt();
    }

    /// <summary>Warehouse-da nomenklatura silinəndə (soft delete) güzgünü deaktiv edir.</summary>
    public void Deactivate()
    {
        IsActive = false;
        SyncedAtUtc = DateTimeOffset.UtcNow;
        SetUpdatedAt();
    }
}
