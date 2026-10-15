using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Payroll;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payroll.Queries.GetPayrollRunByPeriod;

internal sealed class GetPayrollRunByPeriodQueryHandler : IQueryHandler<GetPayrollRunByPeriodQuery, PayrollRunResponse>
{
    private readonly IPayrollRunRepository _payrollRunRepository;

    public GetPayrollRunByPeriodQueryHandler(IPayrollRunRepository payrollRunRepository)
    {
        _payrollRunRepository = payrollRunRepository;
    }

    public async Task<Result<PayrollRunResponse>> Handle(GetPayrollRunByPeriodQuery request, CancellationToken cancellationToken)
    {
        var run = await _payrollRunRepository.GetByPeriodWithPayslipsAsync(request.Year, request.Month, cancellationToken);
        if (run is null)
        {
            return Result.Failure<PayrollRunResponse>(new Error(
                "PayrollRun.NotFound",
                $"{request.Year}-{request.Month:D2} dövrü üçün hesablama hələ aparılmayıb"));
        }

        return Result.Success(PayrollRunResponse.FromDomain(run));
    }
}
