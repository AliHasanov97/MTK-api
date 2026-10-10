using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.EducationLeaveOrders.GetEducationLeaveOrderById;

public sealed record GetEducationLeaveOrderByIdQuery(Guid Id)
    : IQuery<GetEducationLeaveOrderByIdResponse>;
