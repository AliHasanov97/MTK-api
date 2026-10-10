using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.EmploymentStatusChangeOrders;
using MTK.Modules.Hr.Domain.Orders;

namespace MTK.Modules.Hr.Domain.EmploymentStatusChangeApplications;

public sealed class EmploymentStatusChangeApplication : Application
{
    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = null!;

    public override Employee? RelatedEmployee => Employee;

    public EmploymentType CurrentEmploymentType { get; private set; }
    public EmploymentType NewEmploymentType { get; private set; }

    public Guid OrderExecutionSupervisorId { get; private set; }
    public Employee OrderExecutionSupervisor { get; private set; } = null!;

    public EmploymentStatusChangeOrder? EmploymentStatusChangeOrder { get; private set; }

    public override Order? RelatedOrder => EmploymentStatusChangeOrder;


    private EmploymentStatusChangeApplication() { }

    public static EmploymentStatusChangeApplication Create(
        Guid employeeId,
        EmploymentType currentEmploymentType,
        EmploymentType newEmploymentType,
        Guid orderExecutionSupervisorId,
        Guid createdById)
    {
        var application = new EmploymentStatusChangeApplication
        {
            Id = Guid.NewGuid(),
            Type = ApplicationType.EmploymentStatusChange,
            Status = ApplicationStatus.PendingApproval,
            CreatedById = createdById,
            EmployeeId = employeeId,
            CurrentEmploymentType = currentEmploymentType,
            NewEmploymentType = newEmploymentType,
            OrderExecutionSupervisorId = orderExecutionSupervisorId
        };

        return application;
    }

    public void Update(
        EmploymentType? newEmploymentType,
        Guid? orderExecutionSupervisorId)
    {
        if (newEmploymentType.HasValue) NewEmploymentType = newEmploymentType.Value;
        if (orderExecutionSupervisorId.HasValue) OrderExecutionSupervisorId = orderExecutionSupervisorId.Value;
    }
}