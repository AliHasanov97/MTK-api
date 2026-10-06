using MTK.Common.Domain.Abstractions;
using MTK.Modules.Warehouse.Domain.Nomenclatures.Events;

namespace MTK.Modules.Warehouse.Domain.Nomenclatures;

/// <summary>
/// Nomenklatura - mal və xidmətlərin klassifikasiyası
/// </summary>
public sealed class Nomenclature : SearchableEntity
{
    private Nomenclature(
        Guid id,
        string code,
        string name,
        string? description,
        NomenclatureCategory category,
        Unit unit,
        decimal? minStockLevel,
        bool isActive) : base(id)
    {
        Code = code;
        Name = name;
        Description = description;
        Category = category;
        Unit = unit;
        MinStockLevel = minStockLevel;
        IsActive = isActive;
    }

    // Private constructor for EF Core
    private Nomenclature() : base()
    {
    }

    /// <summary>
    /// Unikal nomenklatura kodu (məs: "NOM-001", "MAT-KABEL-001")
    /// </summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>
    /// Nomenklatura adı
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Təsvir
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Kateqoriya (Material, Avadanlıq, Xidmət və s.)
    /// </summary>
    public NomenclatureCategory Category { get; private set; }

    /// <summary>
    /// Ölçü vahidi (ədəd, metr, kq və s.)
    /// </summary>
    public Unit Unit { get; private set; }

    /// <summary>
    /// Minimum ehtiyat səviyyəsi (bu səviyyədən aşağı düşərsə xəbərdarlıq)
    /// </summary>
    public decimal? MinStockLevel { get; private set; }

    /// <summary>
    /// Aktiv/Deaktiv status
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Yeni nomenklatura yaradır
    /// </summary>
    public static Nomenclature Create(
        string code,
        string name,
        string? description,
        NomenclatureCategory category,
        Unit unit,
        decimal? minStockLevel = null,
        bool isActive = true)
    {
        var nomenclature = new Nomenclature(
            Guid.NewGuid(),
            code,
            name,
            description,
            category,
            unit,
            minStockLevel,
            isActive);

        nomenclature.SetCreatedAt();

        nomenclature.RaiseDomainEvent(new NomenclatureCreatedDomainEvent(nomenclature.Id));

        return nomenclature;
    }

    /// <summary>
    /// Nomenklaturanı yeniləyir
    /// </summary>
    public void Update(
        string name,
        string? description,
        NomenclatureCategory category,
        Unit unit,
        decimal? minStockLevel)
    {
        Name = name;
        Description = description;
        Category = category;
        Unit = unit;
        MinStockLevel = minStockLevel;

        SetUpdatedAt();

        RaiseDomainEvent(new NomenclatureUpdatedDomainEvent(Id));
    }

    /// <summary>
    /// Nomenklatura kodunu yeniləyir (ehtiyatlı!)
    /// </summary>
    public void UpdateCode(string code)
    {
        Code = code;
        SetUpdatedAt();
    }

    /// <summary>
    /// Aktiv edir
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        SetUpdatedAt();
    }

    /// <summary>
    /// Deaktiv edir
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }

    /// <summary>
    /// Soft delete
    /// </summary>
    public void Delete()
    {
        SetDeletedAt();

        RaiseDomainEvent(new NomenclatureDeletedDomainEvent(Id));
    }
}
