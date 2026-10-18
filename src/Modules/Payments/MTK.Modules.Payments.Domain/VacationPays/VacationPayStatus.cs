namespace MTK.Modules.Payments.Domain.VacationPays;

/// <summary>
/// Məzuniyyət haqqının ödəniş statusu.
/// </summary>
public enum VacationPayStatus
{
    /// <summary>
    /// Ödənilməyi gözləyir.
    /// Məzuniyyətdən ən azı 3 gün əvvəl ödənilməlidir.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Ödənilib.
    /// Transaction yaradılıb və şirkət balansından çıxılıb.
    /// </summary>
    Paid = 2
}
