namespace MTK.Modules.Payments.Domain.Parties;

/// <summary>
/// Borcun/ödənişin hansı tərəfə aid olduğunu bildirən discriminator.
///
/// Sakin borc/ödəniş axını ilə tədarükçü borc/ödəniş axını eyni aqreqatda
/// (<see cref="Charges.Charge"/> / <see cref="Payments.Payment"/>) birləşdirilib;
/// tərəfi yalnız bu sahə ayırır. Ona görə ödənişlərin borclara paylanması
/// (allocation / FIFO / avans) hər iki tərəf üçün eyni mexanizmlə işləyir.
/// </summary>
public enum PartyType
{
    /// <summary>Mülk sahibi (sakin).</summary>
    Owner,

    /// <summary>Tədarükçü.</summary>
    Vendor
}
