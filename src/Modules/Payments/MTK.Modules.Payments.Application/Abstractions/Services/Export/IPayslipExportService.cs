namespace MTK.Modules.Payments.Application.Abstractions.Services.Export;

public interface IPayslipExportService
{
    /// <summary>Bir işçi üçün bir qəbz, bir səhifə.</summary>
    MemoryStream ExportToPdf(PayslipExportData data);

    /// <summary>Bir PayrollRun-dakı bütün işçilər üçün — hər biri öz səhifəsində, bir sənəddə.</summary>
    MemoryStream ExportRunToPdf(IReadOnlyCollection<PayslipExportData> payslips);
}

/// <summary>
/// PDF-ə lazım olan hər şey — artıq Payslip entity-sində hesablanmış (bax: PayrollCalculator),
/// burada heç bir yenidən hesablama olmur, yalnız render.
/// </summary>
public sealed record PayslipExportData(
    string CompanyName,
    string EmployeeFullName,
    int Year,
    int Month,
    decimal BaseSalary,
    decimal BonusAmount,
    decimal GrossSalary,
    decimal EmployeeDsmf,
    decimal EmployeeUnemployment,
    decimal EmployeeMedicalInsurance,
    decimal IncomeTax,
    decimal NetSalary,
    decimal CourtDeductionPercentage,
    decimal CourtDeductionAmount,
    decimal FinalNetSalary);
