using MTK.Common.Domain.Abstractions;

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
    public decimal GrossSalary { get; private set; }
    public decimal EmployeeDsmf { get; private set; }
    public decimal EmployeeUnemployment { get; private set; }
    public decimal EmployeeMedicalInsurance { get; private set; }
    public decimal TaxableIncome { get; private set; }
    public decimal IncomeTax { get; private set; }
    public decimal NetSalary { get; private set; }

    public decimal CourtDeductionPercentage { get; private set; }
    public decimal CourtDeductionAmount { get; private set; }
    public decimal FinalNetSalary { get; private set; }

    public decimal EmployerDsmf { get; private set; }
    public decimal EmployerUnemployment { get; private set; }
    public decimal EmployerMedicalInsurance { get; private set; }
    public decimal TotalEmployerCost { get; private set; }

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
            GrossSalary = result.GrossSalary,
            EmployeeDsmf = result.EmployeeDsmf,
            EmployeeUnemployment = result.EmployeeUnemployment,
            EmployeeMedicalInsurance = result.EmployeeMedicalInsurance,
            TaxableIncome = result.TaxableIncome,
            IncomeTax = result.IncomeTax,
            NetSalary = result.NetSalary,
            CourtDeductionPercentage = result.CourtDeductionPercentage,
            CourtDeductionAmount = result.CourtDeductionAmount,
            FinalNetSalary = result.FinalNetSalary,
            EmployerDsmf = result.EmployerDsmf,
            EmployerUnemployment = result.EmployerUnemployment,
            EmployerMedicalInsurance = result.EmployerMedicalInsurance,
            TotalEmployerCost = result.TotalEmployerCost
        };

        payslip.SetCreatedAt();
        return payslip;
    }
}
