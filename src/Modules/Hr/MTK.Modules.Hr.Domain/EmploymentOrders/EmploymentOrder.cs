using MTK.Modules.Hr.Domain.EmploymentOrders.Events;
using MTK.Modules.Hr.Domain.Helpers;
using MTK.Modules.Hr.Domain.JobApplications;
using MTK.Modules.Hr.Domain.LaborCodeCases;
using MTK.Modules.Hr.Domain.Orders;

namespace MTK.Modules.Hr.Domain.EmploymentOrders;

public sealed class EmploymentOrder : Order
{
    public Guid JobApplicationId { get; private set; }
    public JobApplication JobApplication { get; private set; } = null!;

    // Override base property to return JobApplication
    public override JobApplication? RelatedJobApplicant => JobApplication;

    public DateTimeOffset StartDate { get; private set; }
    public DateTimeOffset EndDate { get; private set; }
    public Guid LaborCodeCaseId { get; private set; }
    public LaborCodeCase LaborCodeCase { get; private set; } = null!;

    private EmploymentOrder() { }

    public static EmploymentOrder Create(
        Guid jobApplicationId,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        Guid createdById,
        Guid laborCodeCaseId,
        string name,
        string surname,
        string fathersName,
        Gender gender,
        Guid jobId)
    {
        var employmentOrder = new EmploymentOrder
        {
            Id = Guid.NewGuid(),
            Type = OrderType.Employment,
            CreatedById = createdById,
            JobApplicationId = jobApplicationId,
            StartDate = DateTimeHelper.ToUtcDateOnly(startDate),
            EndDate = DateTimeHelper.ToUtcDateOnly(endDate),
            LaborCodeCaseId = laborCodeCaseId
        };

        employmentOrder.RaiseDomainEvent(new EmploymentOrderCreatedDomainEvent
        {
            EmploymentOrderId = employmentOrder.Id,
            OrderDate = DateTimeHelper.ToUtcDateOnly(startDate),
            Name = name,
            Surname = surname,
            FathersName = fathersName,
            Gender = gender,
            JobApplicationId = jobApplicationId,
            JobId = jobId,
            CreatedById = createdById
        });

        return employmentOrder;
    }
}
