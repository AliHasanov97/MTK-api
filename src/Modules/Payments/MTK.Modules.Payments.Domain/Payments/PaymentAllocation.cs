using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.Payments;

public sealed class PaymentAllocation : Entity
{
    private PaymentAllocation() : base() { }

    public Guid PaymentId { get; private set; }
    public Guid ChargeId { get; private set; }
    public decimal Amount { get; private set; }

    /// <summary>
    /// Bu paylanma tətbiq olunandan dərhal sonra <see cref="ChargeId"/> haqqının
    /// qalıq borcu (<see cref="Charges.Charge.OutstandingAmount"/>, həmin an).
    /// Sonradan dəyişmir: gələcək paylanmalar bu qeydi geri işləməyə təsir
    /// etmir, yalnız özününkini yaradır — bank çıxarışındakı "əməliyyatdan
    /// sonra qalıq" sətri kimi, amma hesab üzrə yox, məhz bu haqq üzrə.
    /// </summary>
    public decimal RemainingDebtAfterPayment { get; private set; }

    /// <summary>
    /// true — bu paylanma bu haqq yaranarkən sahibin/tədarükçünün ƏVVƏLKİ
    /// ödənişlərindən qalan avansdan avtomatik bağlanıb (<see cref="Charges.Charge.ApplyAdvanceFrom"/>).
    /// false — bu paylanma elə bu ödənişin özü daxil olanda birbaşa bu haqqa
    /// tətbiq olunub (<see cref="Charges.Charge.ApplyPayment"/>). Yaranış anında
    /// müəyyənləşir, sonradan dəyişmir.
    /// </summary>
    public bool IsFromAdvance { get; private set; }

    public static PaymentAllocation Create(
        Guid paymentId,
        Guid chargeId,
        decimal amount,
        decimal remainingDebtAfterPayment,
        bool isFromAdvance)
    {
        var allocation = new PaymentAllocation
        {
            PaymentId = paymentId,
            ChargeId = chargeId,
            Amount = amount,
            RemainingDebtAfterPayment = remainingDebtAfterPayment,
            IsFromAdvance = isFromAdvance
        };
        allocation.SetCreatedAt();
        return allocation;
    }

    public void Delete()
    {
        SetDeletedAt();
    }
}