using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.CompensationOrders.GetCompensationOrderById;

public sealed record GetCompensationOrderByIdQuery(Guid Id) : IQuery<GetCompensationOrderByIdResponse>;