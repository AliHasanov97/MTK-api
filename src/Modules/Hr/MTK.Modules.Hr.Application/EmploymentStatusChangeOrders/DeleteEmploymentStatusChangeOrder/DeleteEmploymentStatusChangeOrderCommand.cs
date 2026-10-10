using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.EmploymentStatusChangeOrders.DeleteEmploymentStatusChangeOrder;

public sealed record DeleteEmploymentStatusChangeOrderCommand(Guid Id)
    : ICommand<DeleteEmploymentStatusChangeOrderResponse>;
