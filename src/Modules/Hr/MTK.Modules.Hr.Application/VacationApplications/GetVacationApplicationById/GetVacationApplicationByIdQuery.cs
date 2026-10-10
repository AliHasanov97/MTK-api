using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.VacationApplications.GetVacationApplicationById;

public sealed record GetVacationApplicationByIdQuery(Guid Id)
    : IQuery<GetVacationApplicationByIdResponse>;