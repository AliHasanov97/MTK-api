using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.UnpaidLeaveOrders.DeleteUnpaidLeaveOrder;

public sealed record DeleteUnpaidLeaveOrderCommand(Guid Id) : ICommand;
