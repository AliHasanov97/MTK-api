namespace MTK.Modules.Payments.Domain.SalaryPayments;

/// <summary>
/// Maaş ödənişi line növü.
/// </summary>
public enum SalaryPaymentLineType
{
    /// <summary>Əsas maaş.</summary>
    BaseSalary = 1,

    /// <summary>Bonus.</summary>
    Bonus = 2,

    /// <summary>HR düzəlişi (məzuniyyət, ödənişsiz, kompensasiya).</summary>
    Adjustment = 3
}
