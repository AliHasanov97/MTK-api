using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.VacationReturnApplications.GetVacationReturnApplicationById;

public sealed record GetVacationReturnApplicationByIdQuery(Guid Id)
    : IQuery<GetVacationReturnApplicationByIdResponse>;