using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Helpers;
using MTK.Modules.Hr.Domain.Orders;
using MTK.Modules.Hr.Domain.VacationReturnOrders;

namespace MTK.Modules.Hr.Domain.VacationReturnApplications;

/// <summary>
/// Məzuniyyətdən geri qayıtma ərizəsi
/// </summary>
public sealed class VacationReturnApplication : Application
{
    /// <summary>
    /// İşçi
    /// </summary>
    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = null!;

    /// <summary>
    /// İşə qayıdış tarixi
    /// </summary>
    public DateTimeOffset ReturnDate { get; private set; }

    /// <summary>
    /// Qeydlər
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// Bu ərizədən yaradılmış əmr (nullable - convert olunmamış ərizələrdə null ola bilər)
    /// </summary>
    public VacationReturnOrder? VacationReturnOrder { get; private set; }

    // Override base properties
    public override Employee? RelatedEmployee => Employee;
    public override Order? RelatedOrder => VacationReturnOrder;

    private VacationReturnApplication() { }

    /// <summary>
    /// Məzuniyyətdən geri qayıtma ərizəsi yaradır
    /// </summary>
    public static Result<VacationReturnApplication> Create(
        Guid employeeId,
        DateTimeOffset returnDate,
        Guid createdById,
        string? notes = null)
    {
        var id = Guid.NewGuid();
        var application = new VacationReturnApplication
        {
            Id = id,
            Type = ApplicationType.VacationReturn,
            Status = ApplicationStatus.PendingApproval,
            EmployeeId = employeeId,
            ReturnDate = DateTimeHelper.ToUtcDateOnly(returnDate),
            CreatedById = createdById,
            Notes = notes
        };

        return Result.Success(application);
    }

    /// <summary>
    /// Qeydləri yenilə
    /// </summary>
    public void UpdateNotes(string? notes)
    {
        Notes = notes;
    }

    /// <summary>
    /// İşə qayıdış tarixini yenilə
    /// </summary>
    public Result UpdateReturnDate(DateTimeOffset returnDate)
    {
        // Yalnız PendingApproval statusunda update edilə bilər
        if (Status != ApplicationStatus.PendingApproval)
            return Result.Failure(VacationReturnApplicationErrors.CannotUpdateNonPendingApplication);

        ReturnDate = DateTimeHelper.ToUtcDateOnly(returnDate);
        return Result.Success();
    }
}