using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Application.Payroll;

public sealed record PayslipResponse(
    Guid EmployeeId,
    string EmployeeFullName,
    decimal BaseSalary,
    decimal BonusAmount,
    decimal AdjustmentAmount,
    decimal GrossSalary,
    decimal FinalNetSalary,
    decimal TotalEmployerCost,
    DateTimeOffset? PaidAt,
    PaymentMethod? PaymentMethod);
