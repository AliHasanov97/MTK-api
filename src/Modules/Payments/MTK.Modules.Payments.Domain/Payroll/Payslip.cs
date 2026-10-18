using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.Payroll.Events;

namespace MTK.Modules.Payments.Domain.Payroll;

/// <summary>
/// Bir işçinin bir ay üzrə əmək haqqı hesabatı. PayrollRun aggregate-inin
/// daxilindədir — yalnız <see cref="PayrollRun.AddPayslip"/> ilə yaradılır,
/// çünki təkbaşına mənası yoxdur (ContractService ilə eyni fəlsəfə).
/// </summary>
public sealed class Payslip : Entity
{
    private Payslip() : base() { }

    public Guid PayrollRunId { get; private set; }

    public Guid EmployeeId { get; private set; }

    /// <summary>Hesablama anındakı ad snapshot-u (Employee sonra adını dəyişsə belə, bu qəbz dəyişməz qalır).</summary>
    public string EmployeeFullName { get; private set; } = string.Empty;

    public decimal BaseSalary { get; private set; }
    public decimal BonusAmount { get; private set; }

    /// <summary>HR düzəlişinin pul ekvivalenti (saat əsaslı; mənfi = maaşdan çıxılma).</summary>
    public decimal AdjustmentAmount { get; private set; }

    public decimal GrossSalary { get; private set; }
    public decimal FinalNetSalary { get; private set; }
    public decimal TotalEmployerCost { get; private set; }

    public DateTimeOffset? PaidAt { get; private set; }
    public PaymentMethod? PaymentMethod { get; private set; }

    internal static Payslip Create(Guid payrollRunId, Guid employeeId, string employeeFullName, PayrollCalculationResult result)
    {
        // Id qəsdən təyin edilmir: EF onu özü generasiya edir (bax: ContractService.Create).
        var payslip = new Payslip
        {
            PayrollRunId = payrollRunId,
            EmployeeId = employeeId,
            EmployeeFullName = employeeFullName,
            BaseSalary = result.BaseSalary,
            BonusAmount = result.BonusAmount,
            AdjustmentAmount = result.AdjustmentAmount,
            GrossSalary = result.GrossSalary,
            FinalNetSalary = result.FinalNetSalary,
            TotalEmployerCost = result.TotalEmployerCost
        };

        payslip.SetCreatedAt();
        return payslip;
    }

    /// <summary>
    /// Yenidən hesablama: eyni işçinin mövcud qəbzi <b>yerində</b> yenilənir. Qəbzi silib yenisini
    /// yaratmaq (PayrollRunId, EmployeeId) unikal indeksini pozur — EF silmələrdən əvvəl insert etməyə
    /// çalışır. Yalnız PayrollRun.Recalculate çağırır.
    /// </summary>
    internal void UpdateCalculation(string employeeFullName, PayrollCalculationResult result)
    {
        if (PaidAt.HasValue)
        {
            throw new InvalidOperationException($"Payslip {Id} artıq ödənilib ({PaidAt:yyyy-MM-dd}) — dəyişdirilə bilməz");
        }

        EmployeeFullName = employeeFullName;
        BaseSalary = result.BaseSalary;
        BonusAmount = result.BonusAmount;
        AdjustmentAmount = result.AdjustmentAmount;
        GrossSalary = result.GrossSalary;
        FinalNetSalary = result.FinalNetSalary;
        TotalEmployerCost = result.TotalEmployerCost;
        SetUpdatedAt();
    }

    /// <summary>Yalnız PayrollRun.PayPayslip/PayAllPayslips tərəfindən çağırılır — status qaydası aggregate-dədir.</summary>
    internal void MarkAsPaid(PaymentMethod method, DateTimeOffset paidAt)
    {
        if (PaidAt.HasValue)
        {
            throw new InvalidOperationException($"Payslip {Id} artıq ödənilib ({PaidAt:yyyy-MM-dd})");
        }

        PaymentMethod = method;
        PaidAt = paidAt;
        SetUpdatedAt();
    }
}
