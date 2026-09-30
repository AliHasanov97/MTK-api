namespace MTK.Modules.Payments.Domain.Charges;

public enum ChargeStatus
{
    Unpaid,
    PartiallyPaid,
    Paid,

    /// <summary>
    /// Səhv yaranmış borc ləğv edilib (silinmir — tarixçə qalır, sadəcə ödəniş
    /// tələb etmir). Əvvəllər yalnız tədarükçü borcunda mövcud idi.
    /// </summary>
    Cancelled
}
