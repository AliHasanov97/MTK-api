using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.OwnerBalances;

/// <summary>
/// Sahibin balansının <b>proyeksiyası</b>. Həqiqət mənbəyi borclar (Charge) və
/// tamamlanmış ödənişlərdir (Payment); bu cədvəl yalnız sürətli oxumaq üçündür.
///
/// Vacib: dəyərlər <b>heç vaxt artırılmır</b> — hər dəyişiklikdən sonra aqreqatdan
/// mütləq yenidən hesablanır (<see cref="SetTotals"/>). Artırma (increment) yanaşması
/// bir dənə buraxılmış çağırışda balansı həmişəlik səhv qoyurdu; mütləq yenidən
/// hesablamada isə sürüşmə (drift) struktur olaraq mümkün deyil.
/// </summary>
public sealed class OwnerBalance : SearchableEntity
{
    private OwnerBalance() : base() { }

    public Guid OwnerId { get; private set; }
    public decimal TotalDebt { get; private set; }
    public decimal TotalPaid { get; private set; }
    public decimal CurrentBalance => TotalPaid - TotalDebt;

    /// <summary>Qalıq borc — mənfi balansın müsbət hissəsi, heç vaxt mənfi olmur.</summary>
    public decimal CurrentDebt => Math.Max(0, -CurrentBalance);

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

    /// <summary>
    /// Balansı aqreqatdan gələn mütləq dəyərlərlə yeniləyir. Mənfi dəyər qəbul edilmir.
    /// </summary>
    public void SetTotals(decimal totalDebt, decimal totalPaid)
    {
        if (totalDebt < 0)
            throw new ArgumentException("Ümumi borc mənfi ola bilməz", nameof(totalDebt));

        if (totalPaid < 0)
            throw new ArgumentException("Ümumi ödəniş mənfi ola bilməz", nameof(totalPaid));

        TotalDebt = totalDebt;
        TotalPaid = totalPaid;
        SetUpdatedAt();
    }

    public void Delete()
    {
        SetDeletedAt();
    }
}
