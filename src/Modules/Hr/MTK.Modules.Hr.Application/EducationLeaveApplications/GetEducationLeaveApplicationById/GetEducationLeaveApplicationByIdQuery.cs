using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.EducationLeaveApplications.GetEducationLeaveApplicationById;

public sealed record GetEducationLeaveApplicationByIdQuery(Guid Id)
    : IQuery<GetEducationLeaveApplicationByIdResponse>;
