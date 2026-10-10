using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.ApplicationsForChangeOfPosition.Events;

using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Helpers;
using MTK.Modules.Hr.Domain.Jobs;
using MTK.Modules.Hr.Domain.Orders;
using MTK.Modules.Hr.Domain.OrdersForChangeOfPosition;

namespace MTK.Modules.Hr.Domain.ApplicationsForChangeOfPosition;

public sealed class ApplicationForChangeOfPosition : Application
{
    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = null!;

    // Override base property to return Employee
    public override Employee? RelatedEmployee => Employee;

    public Guid CurrentJobId { get; private set; }
    public Job CurrentJob { get; private set; } = null!;

    public Guid NewJobId { get; private set; }
    public Job NewJob { get; private set; } = null!;

    public DateTimeOffset SetDate { get; private set; }

    /// <summary>
    /// Bu ərizədən yaradılmış əmr (nullable - convert olunmamış ərizələrdə null ola bilər)
    /// </summary>
    public OrderForChangeOfPosition? OrderForChangeOfPosition { get; private set; }

    // Override base property
    public override Order? RelatedOrder => OrderForChangeOfPosition;

    private ApplicationForChangeOfPosition() { }

    public static ApplicationForChangeOfPosition Create(
        Guid employeeId,
        Guid currentJobId,
        Guid newJobId,
        DateTimeOffset setDate,
        Guid createdById)
    {
        var application = new ApplicationForChangeOfPosition
        {
            Id = Guid.NewGuid(),
            Type = ApplicationType.ChangeOfPosition,
            Status = ApplicationStatus.PendingApproval,
            CreatedById = createdById,
            EmployeeId = employeeId,
            CurrentJobId = currentJobId,
            NewJobId = newJobId,
            SetDate = DateTimeHelper.ToUtcDateOnly(setDate)
        };

        application.RaiseDomainEvent(new ApplicationForChangeOfPositionCreatedDomainEvent
        {
            ApplicationId = application.Id,
            ApplicationDate = DateTimeHelper.ToUtcDateOnly(setDate)
        });

        return application;
    }

    public void Update(
        Guid? newJobId,
        DateTimeOffset? setDate)
    {
        if (newJobId.HasValue) NewJobId = newJobId.Value;
        if (setDate.HasValue) SetDate = DateTimeHelper.ToUtcDateOnly(setDate.Value);
    }
}
