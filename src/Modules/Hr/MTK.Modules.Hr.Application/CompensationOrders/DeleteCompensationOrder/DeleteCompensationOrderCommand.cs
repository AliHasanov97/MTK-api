using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.CompensationOrders.DeleteCompensationOrder;

public sealed record DeleteCompensationOrderCommand(Guid Id) : ICommand;