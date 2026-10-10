using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.EmploymentStatusChangeOrders.GetEmploymentStatusChangeOrderById;

public sealed record GetEmploymentStatusChangeOrderByIdQuery(Guid Id)
    : IQuery<GetEmploymentStatusChangeOrderByIdResponse>;
