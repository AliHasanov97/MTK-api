using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Helpers;
using MTK.Modules.Hr.Domain.Orders;
using MTK.Modules.Hr.Domain.UnpaidLeaveApplications;
using MTK.Modules.Hr.Domain.UnpaidLeaveOrders.Events;

namespace MTK.Modules.Hr.Domain.UnpaidLeaveOrders;

/// <summary>
/// Ödənişsiz məzuniyyət əmri/sərəncamı
/// UnpaidLeaveApplication-dan convert edildikdə yaradılır
/// </summary>
public sealed class UnpaidLeaveOrder : Order
{
    /// <summary>
    /// Hansı müraciətdən yaradılıb
    /// </summary>
    public Guid UnpaidLeaveApplicationId { get; private set; }
    public UnpaidLeaveApplication UnpaidLeaveApplication { get; private set; } = null!;

    /// <summary>
    /// İşçi
    /// </summary>
    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = null!;

    /// <summary>
    /// Məzuniyyətin başlama tarixi
    /// </summary>
    public DateTimeOffset StartDate { get; private set; }

    /// <summary>
    /// Məzuniyyətin bitmə tarixi
    /// </summary>
    public DateTimeOffset EndDate { get; private set; }

    /// <summary>
    /// Qeydlər
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// İşə qayıtma tarixi (məzuniyyətdən sonrakı ilk iş günü)
    /// Köhnə order-lər üçün null ola bilər
    /// </summary>
    public DateOnly? ReturnToWorkDate { get; private set; }

    // Override base property
    public override Employee? RelatedEmployee => Employee;

    private UnpaidLeaveOrder() { }

    /// <summary>
    /// Ödənişsiz məzuniyyət əmri yaradır
    /// </summary>
    public static UnpaidLeaveOrder Create(
        Guid unpaidLeaveApplicationId,
        Guid employeeId,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        DateOnly returnToWorkDate,
        Guid createdById,
        string? notes = null)
    {
        var id = Guid.NewGuid();
        var order = new UnpaidLeaveOrder
        {
            Id = id,
            Type = OrderType.UnpaidLeave,
            UnpaidLeaveApplicationId = unpaidLeaveApplicationId,
            EmployeeId = employeeId,
            StartDate = DateTimeHelper.ToUtcDateOnly(startDate),
            EndDate = DateTimeHelper.ToUtcDateOnly(endDate),
            ReturnToWorkDate = returnToWorkDate,
            CreatedById = createdById,
            Notes = notes
        };

        order.RaiseDomainEvent(new UnpaidLeaveOrderCreatedDomainEvent
        {
            OrderId = id,
            EmployeeId = employeeId,
            StartDate = DateTimeHelper.ToUtcDateOnly(startDate),
            EndDate = DateTimeHelper.ToUtcDateOnly(endDate)
        });

        return order;
    }
}
