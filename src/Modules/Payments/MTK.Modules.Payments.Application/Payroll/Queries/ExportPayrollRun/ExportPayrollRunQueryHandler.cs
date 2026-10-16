using MTK.Common.Application.Exporting;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Services.Export;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payroll.Queries.ExportPayrollRun;

internal sealed class ExportPayrollRunQueryHandler : IQueryHandler<ExportPayrollRunQuery, ExportFileResult>
{
    private readonly IPayrollRunRepository _payrollRunRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IPayslipExportService _exportService;

    public ExportPayrollRunQueryHandler(
        IPayrollRunRepository payrollRunRepository,
        ICompanyRepository companyRepository,
        IPayslipExportService exportService)
    {
        _payrollRunRepository = payrollRunRepository;
        _companyRepository = companyRepository;
        _exportService = exportService;
    }

    public async Task<Result<ExportFileResult>> Handle(ExportPayrollRunQuery request, CancellationToken cancellationToken)
    {
        var run = await _payrollRunRepository.GetWithPayslipsAsync(request.PayrollRunId, cancellationToken);
        if (run is null)
        {
            return Result.Failure<ExportFileResult>(new Error(
                "PayrollRun.NotFound",
                $"PayrollRun tapılmadı: {request.PayrollRunId}"));
        }

        if (run.Payslips.Count == 0)
        {
            return Result.Failure<ExportFileResult>(new Error(
                "PayrollRun.NoPayslips",
                "Bu PayrollRun-da heç bir qəbz yoxdur"));
        }

        var company = await _companyRepository.GetAsync(cancellationToken);
        var companyName = company?.Name ?? "MTK";

        var payslips = run.Payslips
            .Select(p => new PayslipExportData(
                companyName,
                p.EmployeeFullName,
                run.Year,
                run.Month,
                p.BaseSalary,
                p.BonusAmount,
                p.GrossSalary,
                p.FinalNetSalary))
            .OrderBy(p => p.EmployeeFullName)
            .ToList();

        var stream = _exportService.ExportRunToPdf(payslips);
        var fileName = $"Emek_haqqi_{run.Year}-{run.Month:D2}.pdf";

        return new ExportFileResult(stream.ToArray(), fileName, "application/pdf");
    }
}
