using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payroll.Queries.GetAllPayrollRuns;

internal sealed class GetAllPayrollRunsQueryHandler
    : IQueryHandler<GetAllPayrollRunsQuery, IEnumerable<PayrollRunListItemResponse>>
{
    private readonly IPayrollRunRepository _payrollRunRepository;

    public GetAllPayrollRunsQueryHandler(IPayrollRunRepository payrollRunRepository)
    {
        _payrollRunRepository = payrollRunRepository;
    }

    public async Task<Result<IEnumerable<PayrollRunListItemResponse>>> Handle(
        GetAllPayrollRunsQuery request,
        CancellationToken cancellationToken)
    {
        var payrollRuns = await _payrollRunRepository.GetAllAsync(cancellationToken);

        var response = payrollRuns
            .Select(PayrollRunListItemResponse.FromDomain)
            .AsEnumerable();

        return Result.Success(response);
    }
}
