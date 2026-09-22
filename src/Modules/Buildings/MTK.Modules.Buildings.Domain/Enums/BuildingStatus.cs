namespace MTK.Modules.Buildings.Domain.Enums;

/// <summary>
/// Binanın statusu
/// </summary>
public enum BuildingStatus
{
    /// <summary>
    /// Aktiv - istifadədədir
    /// </summary>
    Active = 0,

    /// <summary>
    /// Tikintidə
    /// </summary>
    UnderConstruction = 1,

    /// <summary>
    /// Təmirdə
    /// </summary>
    UnderMaintenance = 2,

    /// <summary>
    /// Deaktiv
    /// </summary>
    Inactive = 3
}
