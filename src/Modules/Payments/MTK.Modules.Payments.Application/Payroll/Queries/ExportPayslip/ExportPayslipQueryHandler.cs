using MTK.Common.Application.Exporting;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Services.Export;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payroll.Queries.ExportPayslip;

internal sealed class ExportPayslipQueryHandler : IQueryHandler<ExportPayslipQuery, ExportFileResult>
{
    private readonly IPayrollRunRepository _payrollRunRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IPayslipExportService _exportService;

    public ExportPayslipQueryHandler(
        IPayrollRunRepository payrollRunRepository,
        ICompanyRepository companyRepository,
        IPayslipExportService exportService)
    {
        _payrollRunRepository = payrollRunRepository;
        _companyRepository = companyRepository;
        _exportService = exportService;
    }

    public async Task<Result<ExportFileResult>> Handle(ExportPayslipQuery request, CancellationToken cancellationToken)
    {
        var run = await _payrollRunRepository.GetWithPayslipsAsync(request.PayrollRunId, cancellationToken);
        if (run is null)
        {
            return Result.Failure<ExportFileResult>(new Error(
                "PayrollRun.NotFound",
                $"PayrollRun tapılmadı: {request.PayrollRunId}"));
        }

        var payslip = run.Payslips.FirstOrDefault(p => p.EmployeeId == request.EmployeeId);
        if (payslip is null)
        {
            return Result.Failure<ExportFileResult>(new Error(
                "Payslip.NotFound",
                $"Bu PayrollRun-da işçi {request.EmployeeId} üçün qəbz tapılmadı"));
        }

        var company = await _companyRepository.GetAsync(cancellationToken);

        var data = new PayslipExportData(
            company?.Name ?? "MTK",
            payslip.EmployeeFullName,
            run.Year,
            run.Month,
            payslip.BaseSalary,
            payslip.BonusAmount,
            payslip.GrossSalary,
            payslip.FinalNetSalary);

        var stream = _exportService.ExportToPdf(data);
        var fileName = $"Qebz_{run.Year}-{run.Month:D2}_{payslip.EmployeeFullName.Replace(' ', '_')}.pdf";

        return new ExportFileResult(stream.ToArray(), fileName, "application/pdf");
    }
}
