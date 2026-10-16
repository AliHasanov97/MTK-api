namespace MTK.Modules.Payments.Domain.SalaryPayments;

/// <summary>
/// Maaş ödənişinin statusu.
/// </summary>
public enum SalaryPaymentStatus
{
    /// <summary>Draft - hələ ödənilməyib, dəyişiklik edilə bilər.</summary>
    Draft = 1,

    /// <summary>Paid - ödənilib, artıq dəyişiklik edilə bilməz.</summary>
    Paid = 2
}
