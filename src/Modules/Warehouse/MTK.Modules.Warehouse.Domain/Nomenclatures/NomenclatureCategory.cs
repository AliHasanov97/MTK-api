namespace MTK.Modules.Warehouse.Domain.Nomenclatures;

/// <summary>
/// Nomenklatura kateqoriyası
/// </summary>
public enum NomenclatureCategory
{
    /// <summary>
    /// Material (Tikinti materialları, kabel, boru və s.)
    /// </summary>
    Material = 1,

    /// <summary>
    /// Avadanlıq (Elektrik cihazları, alətlər və s.)
    /// </summary>
    Equipment = 2,

    /// <summary>
    /// Xidmət (Təmir, texniki xidmət və s.)
    /// </summary>
    Service = 3,

    /// <summary>
    /// Təchizat (Kırtasiyye, təmizlik malları və s.)
    /// </summary>
    Supply = 4,

    /// <summary>
    /// Digər
    /// </summary>
    Other = 99
}
