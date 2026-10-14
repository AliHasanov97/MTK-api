namespace MTK.Modules.Payments.Application.Payroll;

public sealed record PayslipResponse(
    Guid EmployeeId,
    string EmployeeFullName,
    decimal GrossSalary,
    decimal EmployeeDsmf,
    decimal EmployeeUnemployment,
    decimal EmployeeMedicalInsurance,
    decimal TaxableIncome,
    decimal IncomeTax,
    decimal NetSalary,
    decimal EmployerDsmf,
    decimal EmployerUnemployment,
    decimal EmployerMedicalInsurance,
    decimal TotalEmployerCost);
