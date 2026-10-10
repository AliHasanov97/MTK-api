using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.UnpaidLeaveApplications.GetUnpaidLeaveApplicationById;

public sealed record GetUnpaidLeaveApplicationByIdQuery(Guid Id)
    : IQuery<GetUnpaidLeaveApplicationByIdResponse>;
