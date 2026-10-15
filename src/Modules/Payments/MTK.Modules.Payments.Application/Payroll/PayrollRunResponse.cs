using MTK.Modules.Payments.Domain.Payroll;

namespace MTK.Modules.Payments.Application.Payroll;

public sealed record PayrollRunResponse(
    Guid Id,
    int Year,
    int Month,
    PayrollRunStatus Status,
    DateTimeOffset? CalculatedAt,
    DateTimeOffset? ApprovedAt,
    DateTimeOffset? PaidAt,
    decimal TotalGross,
    decimal TotalNet,
    decimal TotalEmployerCost,
    IReadOnlyCollection<PayslipResponse> Payslips)
{
    public static PayrollRunResponse FromDomain(PayrollRun run)
    {
        return new PayrollRunResponse(
            run.Id,
            run.Year,
            run.Month,
            run.Status,
            run.CalculatedAt,
            run.ApprovedAt,
            run.PaidAt,
            run.TotalGross,
            run.TotalNet,
            run.TotalEmployerCost,
            run.Payslips
                .Select(p => new PayslipResponse(
                    p.EmployeeId,
                    p.EmployeeFullName,
                    p.BaseSalary,
                    p.BonusAmount,
                    p.AdjustmentAmount,
                    p.GrossSalary,
                    p.EmployeeDsmf,
                    p.EmployeeUnemployment,
                    p.EmployeeMedicalInsurance,
                    p.TaxableIncome,
                    p.IncomeTax,
                    p.NetSalary,
                    p.CourtDeductionPercentage,
                    p.CourtDeductionAmount,
                    p.FinalNetSalary,
                    p.EmployerDsmf,
                    p.EmployerUnemployment,
                    p.EmployerMedicalInsurance,
                    p.TotalEmployerCost,
                    p.PaidAt,
                    p.PaymentMethod))
                .ToList());
    }
}
