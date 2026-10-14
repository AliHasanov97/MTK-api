using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.VacationBalances.Services;

namespace MTK.Modules.Hr.Application.VacationBalances.Queries.GetVacationBalanceByEmployeeId;

internal sealed class GetVacationBalanceByEmployeeIdQueryHandler
    : IQueryHandler<GetVacationBalanceByEmployeeIdQuery, VacationBalanceResponse>
{
    private readonly IVacationBalanceService _vacationBalanceService;

    public GetVacationBalanceByEmployeeIdQueryHandler(IVacationBalanceService vacationBalanceService)
    {
        _vacationBalanceService = vacationBalanceService;
    }

    public async Task<Result<VacationBalanceResponse>> Handle(
        GetVacationBalanceByEmployeeIdQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _vacationBalanceService.EnsureCurrentAsync(request.EmployeeId, request.AsOfDate, cancellationToken);
        if (result.IsFailure)
        {
            return Result.Failure<VacationBalanceResponse>(result.Error);
        }

        return Result.Success(VacationBalanceResponse.FromDomain(result.Value));
    }
}
