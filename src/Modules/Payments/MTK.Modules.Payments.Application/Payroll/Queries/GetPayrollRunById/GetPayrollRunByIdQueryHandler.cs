using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Payroll;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payroll.Queries.GetPayrollRunById;

internal sealed class GetPayrollRunByIdQueryHandler : IQueryHandler<GetPayrollRunByIdQuery, PayrollRunResponse>
{
    private readonly IPayrollRunRepository _payrollRunRepository;

    public GetPayrollRunByIdQueryHandler(IPayrollRunRepository payrollRunRepository)
    {
        _payrollRunRepository = payrollRunRepository;
    }

    public async Task<Result<PayrollRunResponse>> Handle(GetPayrollRunByIdQuery request, CancellationToken cancellationToken)
    {
        var run = await _payrollRunRepository.GetWithPayslipsAsync(request.Id, cancellationToken);
        if (run is null)
        {
            return Result.Failure<PayrollRunResponse>(new Error(
                "PayrollRun.NotFound",
                $"PayrollRun tapılmadı: {request.Id}"));
        }

        return Result.Success(PayrollRunResponse.FromDomain(run));
    }
}
