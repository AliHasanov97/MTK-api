using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Application.Payroll;

public sealed record PayslipResponse(
    Guid EmployeeId,
    string EmployeeFullName,
    decimal BaseSalary,
    decimal BonusAmount,
    decimal AdjustmentAmount,
    decimal GrossSalary,
    decimal EmployeeDsmf,
    decimal EmployeeUnemployment,
    decimal EmployeeMedicalInsurance,
    decimal TaxableIncome,
    decimal IncomeTax,
    decimal NetSalary,
    decimal CourtDeductionPercentage,
    decimal CourtDeductionAmount,
    decimal FinalNetSalary,
    decimal EmployerDsmf,
    decimal EmployerUnemployment,
    decimal EmployerMedicalInsurance,
    decimal TotalEmployerCost,
    DateTimeOffset? PaidAt,
    PaymentMethod? PaymentMethod);
