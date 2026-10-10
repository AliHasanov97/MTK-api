using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.BonusOrders.AddBonusOrder;

public sealed class AddBonusOrderCommand : ICommand<AddBonusOrderResponse>
{
    public Guid EmployeeId { get; set; }
    public Guid OrderExecutionSupervisorId { get; set; }
    public int BonusQuantity { get; set; }
    public int SalaryMonth { get; set; }
    public int SalaryYear { get; set; }
}
