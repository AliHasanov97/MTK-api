namespace MTK.Modules.Buildings.Domain.Enums;

/// <summary>
/// Qaraj və ya dayanacaq növü
/// </summary>
public enum GarageType
{
    /// <summary>
    /// Açıq dayanacaq - standart tərifə görə
    /// </summary>
    OpenParking = 0,

    /// <summary>
    /// Örtülü qaraj - premium tərifə görə
    /// </summary>
    CoveredGarage = 1,

    /// <summary>
    /// Anbar yeri - premium tərifə görə
    /// </summary>
    Storage = 2
}
