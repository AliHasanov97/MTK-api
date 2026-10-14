using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.Payroll;

/// <summary>
/// Bir ay üzrə bütün işçilərin əmək haqqı hesablamasının toplusu (Aggregate Root).
/// Qəbzlər (<see cref="Payslip"/>) yalnız bu aggregate daxilindən əlavə olunur —
/// Contract/ContractService ilə eyni fəlsəfə.
/// </summary>
public sealed class PayrollRun : Entity
{
    private readonly List<Payslip> _payslips = new();

    private PayrollRun() : base() { }

    public int Year { get; private set; }

    /// <summary>1-12.</summary>
    public int Month { get; private set; }

    public PayrollRunStatus Status { get; private set; }

    public DateTimeOffset? CalculatedAt { get; private set; }
    public DateTimeOffset? ApprovedAt { get; private set; }
    public DateTimeOffset? PaidAt { get; private set; }

    public IReadOnlyCollection<Payslip> Payslips => _payslips.AsReadOnly();

    public decimal TotalGross => _payslips.Sum(p => p.GrossSalary);
    public decimal TotalNet => _payslips.Sum(p => p.FinalNetSalary);
    public decimal TotalEmployerCost => _payslips.Sum(p => p.TotalEmployerCost);

    public static PayrollRun Create(int year, int month)
    {
        if (month is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(month), "Ay 1-12 arasında olmalıdır");
        }

        // Id qəsdən təyin edilmir: EF onu özü generasiya edir (bax: Contract.Create).
        var run = new PayrollRun
        {
            Year = year,
            Month = month,
            Status = PayrollRunStatus.Draft
        };

        run.SetCreatedAt();
        return run;
    }

    public void AddPayslip(Guid employeeId, string employeeFullName, PayrollCalculationResult result)
    {
        if (Status != PayrollRunStatus.Draft)
        {
            throw new InvalidOperationException($"PayrollRun {Id} artıq '{Status}' statusundadır — qəbz əlavə edilə bilməz");
        }

        if (_payslips.Any(p => p.EmployeeId == employeeId))
        {
            throw new InvalidOperationException($"Employee {employeeId} üçün bu dövrdə artıq qəbz mövcuddur");
        }

        _payslips.Add(Payslip.Create(Id, employeeId, employeeFullName, result));
        SetUpdatedAt();
    }

    public void MarkCalculated()
    {
        if (Status != PayrollRunStatus.Draft)
        {
            throw new InvalidOperationException($"PayrollRun {Id} '{Status}' statusundadır — yalnız Draft-dan Calculated-a keçə bilər");
        }

        if (_payslips.Count == 0)
        {
            throw new InvalidOperationException("Heç bir qəbz yoxdur — hesablama tamamlana bilməz");
        }

        Status = PayrollRunStatus.Calculated;
        CalculatedAt = DateTimeOffset.UtcNow;
        SetUpdatedAt();
    }

    public void Approve()
    {
        if (Status != PayrollRunStatus.Calculated)
        {
            throw new InvalidOperationException($"PayrollRun {Id} '{Status}' statusundadır — yalnız Calculated-dan Approved-a keçə bilər");
        }

        Status = PayrollRunStatus.Approved;
        ApprovedAt = DateTimeOffset.UtcNow;
        SetUpdatedAt();
    }

    public void MarkPaid()
    {
        if (Status != PayrollRunStatus.Approved)
        {
            throw new InvalidOperationException($"PayrollRun {Id} '{Status}' statusundadır — yalnız Approved-dan Paid-a keçə bilər");
        }

        Status = PayrollRunStatus.Paid;
        PaidAt = DateTimeOffset.UtcNow;
        SetUpdatedAt();
    }
}
