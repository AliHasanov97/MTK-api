using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.VacationCompensationApplications.GetVacationCompensationApplicationById;

public sealed record GetVacationCompensationApplicationByIdQuery(Guid Id)
    : IQuery<GetVacationCompensationApplicationByIdResponse>;