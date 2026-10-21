namespace MTK.Modules.Payments.Application.Abstractions.Services.Export;

public interface IPayslipExportService
{
    /// <summary>Bir işçi üçün bir qəbz, bir səhifə.</summary>
    MemoryStream ExportToPdf(PayslipExportData data);

    /// <summary>Bir PayrollRun-dakı bütün işçilər üçün — hər biri öz səhifəsində, bir sənəddə.</summary>
    MemoryStream ExportRunToPdf(PayrollRunExportData report);
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
    decimal FinalNetSalary);

public sealed record PayrollRunExportData(
    string CompanyName,
    int Year,
    int Month,
    string Status,
    IReadOnlyCollection<PayrollRunEmployeeRow> Employees);

public sealed record PayrollRunEmployeeRow(
    string EmployeeFullName,
    decimal BaseSalary,
    decimal BonusAmount,
    decimal AdjustmentAmount,
    decimal GrossSalary,
    decimal FinalNetSalary,
    decimal TotalEmployerCost,
    DateTimeOffset? PaidAt,
    string? PaymentMethod);
