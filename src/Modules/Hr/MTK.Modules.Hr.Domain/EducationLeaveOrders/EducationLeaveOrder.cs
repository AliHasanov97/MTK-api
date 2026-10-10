using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Helpers;
using MTK.Modules.Hr.Domain.Orders;
using MTK.Modules.Hr.Domain.EducationLeaveApplications;
using MTK.Modules.Hr.Domain.EducationLeaveOrders.Events;

namespace MTK.Modules.Hr.Domain.EducationLeaveOrders;

/// <summary>
/// Təhsil məzuniyyəti əmri/sərəncamı
/// EducationLeaveApplication-dan convert edildikdə yaradılır
/// </summary>
public sealed class EducationLeaveOrder : Order
{
    /// <summary>
    /// Hansı müraciətdən yaradılıb
    /// </summary>
    public Guid EducationLeaveApplicationId { get; private set; }
    public EducationLeaveApplication EducationLeaveApplication { get; private set; } = null!;

    /// <summary>
    /// İşçi
    /// </summary>
    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = null!;

    /// <summary>
    /// Məzuniyyət səbəbi
    /// </summary>
    public string? Reason { get; private set; }

    /// <summary>
    /// Məzuniyyətin başlama tarixi
    /// </summary>
    public DateTimeOffset StartDate { get; private set; }

    /// <summary>
    /// Məzuniyyətin bitmə tarixi
    /// </summary>
    public DateTimeOffset EndDate { get; private set; }

    /// <summary>
    /// İşə qayıtma tarixi (məzuniyyətdən sonrakı ilk iş günü)
    /// Köhnə order-lər üçün null ola bilər
    /// </summary>
    public DateOnly? ReturnToWorkDate { get; private set; }

    // Override base property
    public override Employee? RelatedEmployee => Employee;

    private EducationLeaveOrder() { }

    /// <summary>
    /// Təhsil məzuniyyəti əmri yaradır
    /// </summary>
    public static EducationLeaveOrder Create(
        Guid educationLeaveApplicationId,
        Guid employeeId,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        DateOnly returnToWorkDate,
        Guid createdById,
        string? reason = null)
    {
        var id = Guid.NewGuid();
        var order = new EducationLeaveOrder
        {
            Id = id,
            Type = OrderType.EducationLeave,
            EducationLeaveApplicationId = educationLeaveApplicationId,
            EmployeeId = employeeId,
            Reason = reason,
            StartDate = DateTimeHelper.ToUtcDateOnly(startDate),
            EndDate = DateTimeHelper.ToUtcDateOnly(endDate),
            ReturnToWorkDate = returnToWorkDate,
            CreatedById = createdById
        };

        order.RaiseDomainEvent(new EducationLeaveOrderCreatedDomainEvent
        {
            OrderId = id,
            EmployeeId = employeeId,
            StartDate = DateTimeHelper.ToUtcDateOnly(startDate),
            EndDate = DateTimeHelper.ToUtcDateOnly(endDate)
        });

        return order;
    }
}