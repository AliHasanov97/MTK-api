using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.VacationBalances.Queries.GetVacationBalanceByEmployeeId;
using MTK.Modules.Hr.Domain.VacationBalances;

namespace MTK.Modules.Hr.Application.VacationBalances.Queries.GetAllVacationBalancesByEmployeeId;

internal sealed class GetAllVacationBalancesByEmployeeIdQueryHandler
    : IQueryHandler<GetAllVacationBalancesByEmployeeIdQuery, List<VacationBalanceResponse>>
{
    private readonly IVacationBalanceRepository _vacationBalanceRepository;

    public GetAllVacationBalancesByEmployeeIdQueryHandler(IVacationBalanceRepository vacationBalanceRepository)
    {
        _vacationBalanceRepository = vacationBalanceRepository;
    }

    public async Task<Result<List<VacationBalanceResponse>>> Handle(
        GetAllVacationBalancesByEmployeeIdQuery request,
        CancellationToken cancellationToken)
    {
        var balances = await _vacationBalanceRepository.GetAllByEmployeeIdAsync(request.EmployeeId, cancellationToken);

        var responses = balances.Select(VacationBalanceResponse.FromDomain).ToList();

        return Result.Success(responses);
    }
}
