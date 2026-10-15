using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.Payroll;

/// <summary>
/// HR-dəki SalaryDeduction-ın bu moduldakı snapshot-u (Id = HR SalaryDeduction.Id).
/// Məhkəmə qərarı ilə hər ay net əmək haqqından tutulan faiz — StartDate-dən
/// etibarən, silinənə qədər (borc ödənilib/qərar ləğv olunub) hər ay tətbiq olunur.
/// Silinmə HR-dəki SalaryDeductionDeleted event-i ilə bu snapshot-un özünün
/// (soft-)silinməsi ilə əks olunur.
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

    public static PayrollDeduction Create(Guid id, Guid employeeId, decimal percentageSalary, DateTimeOffset startDate)
    {
        var deduction = new PayrollDeduction(id, employeeId, percentageSalary, startDate);
        deduction.SetCreatedAt();
        return deduction;
    }
}
