using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.EducationLeaveOrders.DeleteEducationLeaveOrder;

public sealed record DeleteEducationLeaveOrderCommand(Guid Id) : ICommand;
