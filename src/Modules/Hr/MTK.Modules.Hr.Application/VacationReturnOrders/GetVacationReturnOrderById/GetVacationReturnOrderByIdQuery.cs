using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.VacationReturnOrders.GetVacationReturnOrderById;

public sealed record GetVacationReturnOrderByIdQuery(Guid Id)
    : IQuery<GetVacationReturnOrderByIdResponse>;