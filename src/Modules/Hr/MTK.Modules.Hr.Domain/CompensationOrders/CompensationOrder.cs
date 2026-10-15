using MTK.Modules.Hr.Domain.CompensationOrders.Events;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.FileAttachments;
using MTK.Modules.Hr.Domain.Orders;

using MTK.Modules.Hr.Domain.VacationCompensationApplications;

namespace MTK.Modules.Hr.Domain.CompensationOrders;

/// <summary>
/// Məzuniyyət kompensasiyası əmri/sərəncamı
/// VacationCompensationApplication-dan convert edildikdə yaradılır
/// </summary>
public sealed class CompensationOrder : Order
{
    /// <summary>
    /// Hansı müraciətdən yaradılıb
    /// </summary>
    public Guid CompensationApplicationId { get; private set; }
    public VacationCompensationApplication CompensationApplication { get; private set; } = null!;

    /// <summary>
    /// İşçi
    /// </summary>
    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = null!;

    /// <summary>
    /// Neçə gün kompensasiya edilir
    /// </summary>
    public int CompensatedDays { get; private set; }

    /// <summary>
    /// Kompensasiya ediləcək iş ili (ərizədən köçürülür)
    /// </summary>
    public DateOnly? WorkYearStart { get; private set; }
    public DateOnly? WorkYearEnd { get; private set; }

    /// <summary>
    /// Qeydlər
    /// </summary>
    public string? Notes { get; private set; }

// Override base property
    public override Employee? RelatedEmployee => Employee;

    private CompensationOrder() { }

    /// <summary>
    /// Kompensasiya əmri yaradır
    /// </summary>
    public static CompensationOrder Create(
        Guid compensationApplicationId,
        Guid employeeId,
        int compensatedDays,
        Guid createdById,
        string? notes = null,
        DateOnly? workYearStart = null,
        DateOnly? workYearEnd = null)
    {
        var order = new CompensationOrder
        {
            Id = Guid.NewGuid(),
            Type = OrderType.VacationCompensation,
            CompensationApplicationId = compensationApplicationId,
            EmployeeId = employeeId,
            CompensatedDays = compensatedDays,
            WorkYearStart = workYearStart,
            WorkYearEnd = workYearEnd,
            CreatedById = createdById,
            Notes = notes
        };

        // İstifadə olunmamış məzuniyyət günlərinin pul kompensasiyasıdır — işçiyə ödəniləcək
        // məbləğ olduğu üçün Payments-in əmək haqqı hesablamasına ötürülməlidir (bax: domain event handler).
        order.RaiseDomainEvent(new CompensationOrderCreatedDomainEvent
        {
            OrderId = order.Id,
            EmployeeId = employeeId,
            CompensatedDays = compensatedDays,
            WorkYearStart = workYearStart,
            WorkYearEnd = workYearEnd
        });

        return order;
    }
}