namespace MTK.Modules.Buildings.Domain.Enums;

/// <summary>
/// Mənzilin statusu
/// </summary>
public enum ApartmentStatus
{
    /// <summary>
    /// Aktiv - sahibi var, istifadə olunur
    /// </summary>
    Active = 0,

    /// <summary>
    /// Boş - hələ satılmayıb
    /// </summary>
    Vacant = 1,

    /// <summary>
    /// Transfer prosesində
    /// </summary>
    InTransfer = 2,

    /// <summary>
    /// Deaktiv - istifadə olunmur
    /// </summary>
    Inactive = 3
}
