namespace MTK.Modules.Payments.Domain.Payroll;

/// <summary>
/// <see cref="PayrollCalculator"/>-ın saf nəticəsi — bir işçi üçün bir ay üzrə
/// əmək haqqı hesablamasının bütün komponentləri. Entity deyil, sadəcə dəyər.
/// </summary>
public sealed record PayrollCalculationResult(
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
