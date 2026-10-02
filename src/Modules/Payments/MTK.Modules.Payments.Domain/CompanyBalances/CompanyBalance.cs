using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.CompanyBalances;

/// <summary>
/// Şirkətin (MTK-nin) ümumi pul balansının <b>proyeksiyası</b> — həqiqət mənbəyi
/// Transaction jurnalıdır (bütün gəlir/xərc qeydləri); bu cədvəl yalnız sürətli
/// oxumaq üçündür. Sahib balansından (<c>OwnerBalance</c>) fərqli olaraq, bu tək
/// sətirlik aqreqatdır — bütün şirkət üçün bir balans (OwnerId yoxdur).
///
/// OwnerBalance ilə eyni qayda: dəyərlər heç vaxt artırılmır (increment) — hər
/// dəyişiklikdən sonra Transaction jurnalından mütləq yenidən hesablanır
/// (<see cref="SetTotals"/>). Artırma yanaşması bir buraxılmış çağırışda balansı
/// həmişəlik səhv qoyurdu; mütləq yenidən hesablamada sürüşmə mümkün deyil.
/// </summary>
public sealed class CompanyBalance : Entity
{
    private CompanyBalance() : base() { }

    public decimal TotalIncome { get; private set; }
    public decimal TotalExpense { get; private set; }
    public decimal CurrentBalance => TotalIncome - TotalExpense;

    public static CompanyBalance Create()
    {
        var balance = new CompanyBalance
        {
            TotalIncome = 0,
            TotalExpense = 0
        };
        balance.SetCreatedAt();
        return balance;
    }

    public void SetTotals(decimal totalIncome, decimal totalExpense)
    {
        if (totalIncome < 0)
            throw new ArgumentException("Ümumi gəlir mənfi ola bilməz", nameof(totalIncome));

        if (totalExpense < 0)
            throw new ArgumentException("Ümumi xərc mənfi ola bilməz", nameof(totalExpense));

        TotalIncome = totalIncome;
        TotalExpense = totalExpense;
        SetUpdatedAt();
    }
}
