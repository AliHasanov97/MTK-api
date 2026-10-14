using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.Payroll;

/// <summary>
/// HR-dəki BonusOrder-in bu moduldakı snapshot-u (Id = HR BonusOrder.Id).
/// Konkret bir ay üçün bir dəfəlik mükafat — <see cref="CalculatePayrollRun"/>
/// zamanı həmin ayın gross-una əlavə olunur. HR-də bonus əmrləri dəyişdirilə/
/// silinə bilmədiyi üçün (yalnız yaradılır) burada da yalnız <see cref="Create"/> var.
/// </summary>
public sealed class PayrollBonus : Entity
{
    private PayrollBonus(Guid id, Guid employeeId, decimal amount, int salaryMonth, int salaryYear) : base(id)
    {
        EmployeeId = employeeId;
        Amount = amount;
        SalaryMonth = salaryMonth;
        SalaryYear = salaryYear;
    }

    private PayrollBonus() : base() { }

    public Guid EmployeeId { get; private set; }
    public decimal Amount { get; private set; }
    public int SalaryMonth { get; private set; }
    public int SalaryYear { get; private set; }

    public static PayrollBonus Create(Guid id, Guid employeeId, decimal amount, int salaryMonth, int salaryYear)
    {
        var bonus = new PayrollBonus(id, employeeId, amount, salaryMonth, salaryYear);
        bonus.SetCreatedAt();
        return bonus;
    }
}
