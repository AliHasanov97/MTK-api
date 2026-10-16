namespace MTK.Modules.Payments.Application.Abstractions.Services.Export;

/// <summary>
/// Maaş ödənişi PDF export service.
/// </summary>
public interface ISalaryPaymentExportService
{
    MemoryStream ExportToPdf(SalaryPaymentExportData data);
}

/// <summary>
/// Maaş ödənişi export data.
/// </summary>
public sealed record SalaryPaymentExportData(
    string CompanyName,
    string EmployeeFullName,
    int Month,
    int Year,
    decimal BaseSalary,
    IReadOnlyCollection<SalaryPaymentLineExportData> Lines,
    decimal GrossSalary,
    string PaymentMethod,
    DateTimeOffset PaymentDate,
    string? Notes);

/// <summary>
/// Maaş ödənişi line export data.
/// </summary>
public sealed record SalaryPaymentLineExportData(
    string LineType,
    string Description,
    decimal Amount);
