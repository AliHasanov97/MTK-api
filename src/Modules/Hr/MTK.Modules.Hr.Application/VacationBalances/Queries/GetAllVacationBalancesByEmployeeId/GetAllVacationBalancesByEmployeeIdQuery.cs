using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Application.VacationBalances.Queries.GetVacationBalanceByEmployeeId;

namespace MTK.Modules.Hr.Application.VacationBalances.Queries.GetAllVacationBalancesByEmployeeId;

public sealed record GetAllVacationBalancesByEmployeeIdQuery(Guid EmployeeId)
    : IQuery<List<VacationBalanceResponse>>;
