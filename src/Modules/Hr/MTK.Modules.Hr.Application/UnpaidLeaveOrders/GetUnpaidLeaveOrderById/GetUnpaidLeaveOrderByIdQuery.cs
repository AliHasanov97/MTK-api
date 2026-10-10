using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.UnpaidLeaveOrders.GetUnpaidLeaveOrderById;

public sealed record GetUnpaidLeaveOrderByIdQuery(Guid Id)
    : IQuery<GetUnpaidLeaveOrderByIdResponse>;
