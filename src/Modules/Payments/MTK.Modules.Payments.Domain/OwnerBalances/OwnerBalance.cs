using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.OwnerBalances;

public sealed class OwnerBalance : SearchableEntity
{
    private OwnerBalance() : base() { }

    public Guid OwnerId { get; private set; }
    public decimal TotalDebt { get; private set; }
    public decimal TotalPaid { get; private set; }
    public decimal CurrentBalance => TotalPaid - TotalDebt;

    public static OwnerBalance Create(Guid ownerId)
    {
        var balance = new OwnerBalance
        {
            OwnerId = ownerId,
            TotalDebt = 0,
            TotalPaid = 0
        };
        balance.SetCreatedAt();
        return balance;
    }

    public void AddCharge(decimal amount)
    {
        TotalDebt += amount;
        SetUpdatedAt();
    }

    public void AddPayment(decimal amount)
    {
        TotalPaid += amount;
        SetUpdatedAt();
    }

    /// <summary>Undoes a previously recorded payment (used when a payment gets cancelled).</summary>
    public void RemovePayment(decimal amount)
    {
        TotalPaid = Math.Max(0, TotalPaid - amount);
        SetUpdatedAt();
    }

    public void Delete()
    {
        SetDeletedAt();
    }
}