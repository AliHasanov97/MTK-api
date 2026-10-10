using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.EmploymentStatusChangeApplications.GetEmploymentStatusChangeApplicationById;

public sealed record GetEmploymentStatusChangeApplicationByIdQuery(Guid Id)
    : IQuery<GetEmploymentStatusChangeApplicationByIdResponse>;
