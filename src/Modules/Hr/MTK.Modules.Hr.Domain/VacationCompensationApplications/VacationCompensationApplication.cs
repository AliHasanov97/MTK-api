using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.CompensationOrders;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.FileAttachments;
using MTK.Modules.Hr.Domain.Orders;

using MTK.Modules.Hr.Domain.VacationCompensationApplications.Events;

namespace MTK.Modules.Hr.Domain.VacationCompensationApplications;

/// <summary>
/// Məzuniyyət kompensasiyası müraciəti
/// </summary>
public sealed class VacationCompensationApplication : Application
{
    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = null!;

    /// <summary>
    /// Hansı iş ilinin balansından kompensasiya edilir (istifadəçi seçir)
    /// </summary>
    public Guid VacationBalanceId { get; private set; }
    public VacationBalances.VacationBalance VacationBalance { get; private set; } = null!;

    /// <summary>
    /// Neçə gün kompensasiya ediləcək
    /// </summary>
    public int RequestedDays { get; private set; }

    /// <summary>
    /// Qeydlər
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// Bu ərizədən yaradılmış əmr (nullable - convert olunmamış ərizələrdə null ola bilər)
    /// </summary>
    public CompensationOrder? CompensationOrder { get; private set; }

    // Override base properties
    public override Employee? RelatedEmployee => Employee;
    public override Order? RelatedOrder => CompensationOrder;

    private VacationCompensationApplication() { }

    /// <summary>
    /// Məzuniyyət kompensasiyası müraciəti yaradır
        /// </summary>
    public static Result<VacationCompensationApplication> Create(
        Guid employeeId,
        Guid vacationBalanceId,
        int requestedDays,
        Guid createdById,
        string? notes = null)
    {
        if (requestedDays <= 0)
            return Result.Failure<VacationCompensationApplication>(
                VacationCompensationApplicationErrors.InvalidRequestedDays);

        var id = Guid.NewGuid();
        var application = new VacationCompensationApplication
        {
            Id = id,
            Type = ApplicationType.VacationCompensation,
            Status = ApplicationStatus.PendingApproval,
            EmployeeId = employeeId,
            VacationBalanceId = vacationBalanceId,
            RequestedDays = requestedDays,
            CreatedById = createdById,
            Notes = notes
        };

        application.RaiseDomainEvent(new VacationCompensationApplicationCreatedDomainEvent
        {
            ApplicationId = application.Id,
            EmployeeId = employeeId,
            RequestedDays = requestedDays
        });

        return Result.Success(application);
    }

    /// <summary>
    /// Qeydləri yenilə
    /// </summary>
    public void UpdateNotes(string? notes)
    {
        Notes = notes;
    }
}