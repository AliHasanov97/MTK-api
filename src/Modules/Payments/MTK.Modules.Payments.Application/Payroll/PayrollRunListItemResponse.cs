using MTK.Modules.Payments.Domain.Payroll;

namespace MTK.Modules.Payments.Application.Payroll;

public sealed record PayrollRunListItemResponse(
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
    int EmployeeCount)
{
    public static PayrollRunListItemResponse FromDomain(PayrollRun run)
    {
        return new PayrollRunListItemResponse(
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
            run.Payslips.Count);
    }
}
