using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Helpers;
using MTK.Modules.Hr.Domain.Orders;
using MTK.Modules.Hr.Domain.VacationApplications;
using MTK.Modules.Hr.Domain.VacationOrders.Events;

namespace MTK.Modules.Hr.Domain.VacationOrders;

/// <summary>
/// Ödənişli məzuniyyət əmri/sərəncamı
/// VacationApplication-dan convert edildikdə yaradılır
/// </summary>
public sealed class VacationOrder : Order
{
    /// <summary>
    /// Hansı müraciətdən yaradılıb
    /// </summary>
    public Guid VacationApplicationId { get; private set; }
    public VacationApplication VacationApplication { get; private set; } = null!;

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
    /// Neçə gün məzuniyyət
    /// </summary>
    public int VacationDays { get; private set; }

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

    private VacationOrder() { }

    /// <summary>
    /// Ödənişli məzuniyyət əmri yaradır
    /// </summary>
    public static VacationOrder Create(
        Guid vacationApplicationId,
        Guid employeeId,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        int vacationDays,
        DateOnly returnToWorkDate,
        Guid createdById,
        string? notes = null)
    {
        var id = Guid.NewGuid();
        var order = new VacationOrder
        {
            Id = id,
            Type = OrderType.Vacation,
            VacationApplicationId = vacationApplicationId,
            EmployeeId = employeeId,
            StartDate = DateTimeHelper.ToUtcDateOnly(startDate),
            EndDate = DateTimeHelper.ToUtcDateOnly(endDate),
            VacationDays = vacationDays,
            ReturnToWorkDate = returnToWorkDate,
            CreatedById = createdById,
            Notes = notes
        };

        order.RaiseDomainEvent(new VacationOrderCreatedDomainEvent
        {
            OrderId = id,
            EmployeeId = employeeId,
            StartDate = DateTimeHelper.ToUtcDateOnly(startDate),
            EndDate = DateTimeHelper.ToUtcDateOnly(endDate),
            VacationDays = vacationDays
        });

        return order;
    }
}