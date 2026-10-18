using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.Payroll;

namespace MTK.Modules.Payments.Application.Payroll.Queries.GetPayslipsByEmployee;

/// <summary>
/// İşçinin bir ay üçün payslip məlumatları.
/// </summary>
public sealed record EmployeePayslipResponse(
    Guid PayrollRunId,
    int Year,
    int Month,
    PayrollRunStatus PayrollRunStatus,
    decimal BaseSalary,
    decimal BonusAmount,
    decimal AdjustmentAmount,
    decimal GrossSalary,
    decimal FinalNetSalary,
    decimal TotalEmployerCost,
    DateTimeOffset? PaidAt,
    PaymentMethod? PaymentMethod);
