using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Services.Export;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.SalaryPayments.Queries.ExportSalaryPaymentPdf;

internal sealed class ExportSalaryPaymentPdfQueryHandler
    : IQueryHandler<ExportSalaryPaymentPdfQuery, FileExportResult>
{
    private static readonly string[] AzMonthNames =
    [
        "Yanvar", "Fevral", "Mart", "Aprel", "May", "İyun",
        "İyul", "Avqust", "Sentyabr", "Oktyabr", "Noyabr", "Dekabr"
    ];

    private readonly ISalaryPaymentRepository _salaryPaymentRepository;
    private readonly ISalaryPaymentExportService _exportService;

    public ExportSalaryPaymentPdfQueryHandler(
        ISalaryPaymentRepository salaryPaymentRepository,
        ISalaryPaymentExportService exportService)
    {
        _salaryPaymentRepository = salaryPaymentRepository;
        _exportService = exportService;
    }

    public async Task<Result<FileExportResult>> Handle(
        ExportSalaryPaymentPdfQuery request,
        CancellationToken cancellationToken)
    {
        var salaryPayment = await _salaryPaymentRepository.GetByIdWithLinesAsync(
            request.SalaryPaymentId, cancellationToken);

        if (salaryPayment is null)
        {
            return Result.Failure<FileExportResult>(SalaryPaymentErrors.PaymentNotFound);
        }

        var exportData = new SalaryPaymentExportData(
            CompanyName: "Məktəb İdarəetmə Kompleksi", // TODO: Get from company settings
            EmployeeFullName: salaryPayment.EmployeeFullName,
            Month: salaryPayment.SalaryMonth,
            Year: salaryPayment.SalaryYear,
            BaseSalary: salaryPayment.BaseSalary,
            Lines: salaryPayment.Lines.Select(l => new SalaryPaymentLineExportData(
                l.LineType.ToString(),
                l.Description,
                l.Amount)).ToList(),
            GrossSalary: salaryPayment.GrossSalary,
            PaymentMethod: salaryPayment.PaymentMethod.ToString(),
            PaymentDate: salaryPayment.PaymentDate,
            Notes: salaryPayment.Notes);

        var pdfStream = _exportService.ExportToPdf(exportData);
        var content = pdfStream.ToArray();

        var fileName = $"Maas_Qebzi_{salaryPayment.EmployeeFullName.Replace(" ", "_")}_{AzMonthNames[salaryPayment.SalaryMonth - 1]}_{salaryPayment.SalaryYear}.pdf";

        return Result.Success(new FileExportResult(
            content,
            "application/pdf",
            fileName));
    }
}
