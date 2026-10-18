using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payroll.Queries.GetPayslipsByEmployee;

internal sealed class GetPayslipsByEmployeeQueryHandler
    : IQueryHandler<GetPayslipsByEmployeeQuery, IEnumerable<EmployeePayslipResponse>>
{
    private readonly IPayrollRunRepository _payrollRunRepository;

    public GetPayslipsByEmployeeQueryHandler(IPayrollRunRepository payrollRunRepository)
    {
        _payrollRunRepository = payrollRunRepository;
    }

    public async Task<Result<IEnumerable<EmployeePayslipResponse>>> Handle(
        GetPayslipsByEmployeeQuery request,
        CancellationToken cancellationToken)
    {
        var payrollRuns = await _payrollRunRepository
            .GetAllPayrollRunsWithPayslipsByEmployeeAsync(request.EmployeeId, cancellationToken);

        var response = payrollRuns
            .SelectMany(run => run.Payslips.Select(payslip => new EmployeePayslipResponse(
                run.Id,
                run.Year,
                run.Month,
                run.Status,
                payslip.BaseSalary,
                payslip.BonusAmount,
                payslip.AdjustmentAmount,
                payslip.GrossSalary,
                payslip.FinalNetSalary,
                payslip.TotalEmployerCost,
                payslip.PaidAt,
                payslip.PaymentMethod)))
            .OrderByDescending(p => p.Year)
            .ThenByDescending(p => p.Month)
            .AsEnumerable();

        return Result.Success(response);
    }
}
