using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.Payments;

public sealed class PaymentAllocation : Entity
{
    private PaymentAllocation() : base() { }

    public Guid PaymentId { get; private set; }
    public Guid ChargeId { get; private set; }
    public decimal Amount { get; private set; }

    public static PaymentAllocation Create(Guid paymentId, Guid chargeId, decimal amount)
    {
        var allocation = new PaymentAllocation
        {
            PaymentId = paymentId,
            ChargeId = chargeId,
            Amount = amount
        };
        allocation.SetCreatedAt();
        return allocation;
    }
}