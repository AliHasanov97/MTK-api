namespace MTK.Modules.Payments.Domain.PropertyOwnerships;

/// <summary>
/// Mirrors Buildings.Domain.Enums.GarageType. Duplicated locally because Payments
/// must not take a hard dependency on the Buildings module (modules only talk to
/// each other through integration events).
/// </summary>
public enum GarageType
{
    OpenParking,
    CoveredGarage,
    Storage
}
