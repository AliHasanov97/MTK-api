using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.VacationPays.Queries.GetVacationPayById;

namespace MTK.Modules.Payments.Application.VacationPays.Queries.GetVacationPaysByEmployee;

public sealed record GetVacationPaysByEmployeeQuery(Guid EmployeeId) : IQuery<IEnumerable<VacationPayResponse>>;
