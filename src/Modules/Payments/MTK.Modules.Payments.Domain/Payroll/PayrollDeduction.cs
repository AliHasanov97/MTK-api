using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.Payroll;

/// <summary>
/// HR-dəki SalaryDeduction-ın bu moduldakı snapshot-u (Id = HR SalaryDeduction.Id).
/// Məhkəmə qərarı ilə hər ay net əmək haqqından tutulan faiz — StartDate-dən
/// etibarən, silinənə qədər (borc ödənilib/qərar ləğv olunub) hər ay tətbiq olunur.
/// Silinmə HR-dəki SalaryDeductionDeleted event-i ilə bu snapshot-un özünün
/// (soft-)silinməsi ilə əks olunur.
///
/// FIFO qaydası: əvvəlcə dövlət rüsumu tam ödənilir, sonra əsas borc.
/// </summary>
public sealed class PayrollDeduction : Entity
{
    private PayrollDeduction(Guid id, Guid employeeId, decimal percentageSalary, DateTimeOffset startDate) : base(id)
    {
        EmployeeId = employeeId;
        PercentageSalary = percentageSalary;
        // PostgreSQL "timestamp with time zone" yalnız offset 0 (UTC) qəbul edir — HR-dən gələn
        // dəyər lokal offsetlərlə (+04:00 və s.) ola bilər, ona görə UTC-yə normallaşdırılır.
        StartDate = startDate.ToUniversalTime();
    }

    private PayrollDeduction() : base() { }

    public Guid EmployeeId { get; private set; }
    public decimal PercentageSalary { get; private set; }
    public DateTimeOffset StartDate { get; private set; }
    public decimal StateFee { get; private set; }
    public decimal? StateFeeRemaining { get; private set; }
    public decimal? DebtRemaining { get; private set; }

    /// <summary>Ümumi qalan məbləğ (dövlət rüsumu + əsas borc).</summary>
    public decimal? TotalRemaining => (StateFeeRemaining ?? 0) + (DebtRemaining ?? 0);

    public static PayrollDeduction Create(
        Guid id,
        Guid employeeId,
        decimal percentageSalary,
        DateTimeOffset startDate,
        decimal stateFee,
        decimal debt)
    {
        var deduction = new PayrollDeduction(id, employeeId, percentageSalary, startDate);
        deduction.StateFee = stateFee;
        deduction.StateFeeRemaining = stateFee > 0 ? stateFee : null;
        deduction.DebtRemaining = debt > 0 ? debt : null;
        deduction.SetCreatedAt();
        return deduction;
    }

    public void UpdateRemaining(decimal stateFeeRemaining, decimal debtRemaining)
    {
        StateFeeRemaining = stateFeeRemaining > 0 ? stateFeeRemaining : null;
        DebtRemaining = debtRemaining > 0 ? debtRemaining : null;
        SetUpdatedAt();
    }
}
