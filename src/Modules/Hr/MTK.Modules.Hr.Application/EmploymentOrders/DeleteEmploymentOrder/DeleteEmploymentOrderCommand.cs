using MTK.Common.Application.Messaging;
using MediatR;

namespace MTK.Modules.Hr.Application.EmploymentOrders.DeleteEmploymentOrder;

public sealed class DeleteEmploymentOrderCommand : ICommand<Unit>
{
    public DeleteEmploymentOrderCommand(Guid id) => Id = id;
    public Guid Id { get; }
}
