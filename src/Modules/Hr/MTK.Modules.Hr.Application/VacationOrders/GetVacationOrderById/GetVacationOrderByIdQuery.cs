using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.VacationOrders.GetVacationOrderById;

public sealed record GetVacationOrderByIdQuery(Guid Id)
    : IQuery<GetVacationOrderByIdResponse>;