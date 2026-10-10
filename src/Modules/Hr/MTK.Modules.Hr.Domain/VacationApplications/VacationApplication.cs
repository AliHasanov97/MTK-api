using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Helpers;
using MTK.Modules.Hr.Domain.Orders;
using MTK.Modules.Hr.Domain.VacationApplications.Events;
using MTK.Modules.Hr.Domain.VacationOrders;

namespace MTK.Modules.Hr.Domain.VacationApplications;

/// <summary>
/// Ödənişli məzuniyyət müraciəti
/// Bir ərizədə bir neçə iş ilindən məzuniyyət götürülə bilər (many-to-many)
/// </summary>
public sealed class VacationApplication : Application
{
    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = null!;

    /// <summary>
    /// Məzuniyyətin başlama tarixi
    /// </summary>
    public DateTimeOffset StartDate { get; private set; }

    /// <summary>
    /// Məzuniyyətin bitmə tarixi (nullable - handler-də hesablanır)
    /// </summary>
    public DateTimeOffset? EndDate { get; private set; }

    /// <summary>
    /// Ümumi neçə gün məzuniyyət (bayramlar çıxılmaqla hesablanıb saxlanılır)
    /// </summary>
    public int TotalRequestedDays { get; private set; }

    /// <summary>
    /// Qeydlər
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// Bu ərizədən yaradılmış əmr (nullable - convert olunmamış ərizələrdə null ola bilər)
    /// </summary>
    public VacationOrder? VacationOrder { get; private set; }

    // Override base properties
    public override Employee? RelatedEmployee => Employee;
    public override Order? RelatedOrder => VacationOrder;

    private VacationApplication() { }

    /// <summary>
    /// Ödənişli məzuniyyət müraciəti yaradır
    /// </summary>
    public static Result<VacationApplication> Create(
        Guid employeeId,
        DateTimeOffset startDate,
        DateTimeOffset? endDate,
        int totalRequestedDays,
        Guid createdById,
        string? notes = null)
    {
        // Validation: end date must be after start date (when provided)
        if (endDate.HasValue && endDate.Value.Date < startDate.Date)
            return Result.Failure<VacationApplication>(
                new Error("VacationApplication.InvalidDates", "Bitmə tarixi başlama tarixindən əvvəl ola bilməz"));

        // Validation: start date cannot be in the past
        if (startDate.Date < DateTimeOffset.UtcNow.Date)
            return Result.Failure<VacationApplication>(
                new Error("VacationApplication.StartDateInPast", "Başlama tarixi keçmişdə ola bilməz"));

        var id = Guid.NewGuid();
        var application = new VacationApplication
        {
            Id = id,
            Type = ApplicationType.Vacation,
            Status = ApplicationStatus.PendingApproval,
            EmployeeId = employeeId,
            StartDate = DateTimeHelper.ToUtcDateOnly(startDate),
            EndDate = DateTimeHelper.ToUtcDateOnly(endDate),
            TotalRequestedDays = totalRequestedDays,
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
    /// Məzuniyyət tarixlərini yenilə
    /// </summary>
    public Result UpdateDates(DateTimeOffset startDate, DateTimeOffset? endDate, int totalRequestedDays)
    {
        // Yalnız PendingApproval statusunda update edilə bilər
        if (Status != ApplicationStatus.PendingApproval)
            return Result.Failure(
                VacationApplicationErrors.CannotUpdateNonPendingApplication);

        if (endDate.HasValue && endDate.Value.Date < startDate.Date)
            return Result.Failure(
                new Error("VacationApplication.InvalidDates", "Bitmə tarixi başlama tarixindən əvvəl ola bilməz"));

        if (startDate.Date < DateTimeOffset.UtcNow.Date)
            return Result.Failure(
                new Error("VacationApplication.StartDateInPast", "Başlama tarixi keçmişdə ola bilməz"));

        StartDate = DateTimeHelper.ToUtcDateOnly(startDate);
        EndDate = DateTimeHelper.ToUtcDateOnly(endDate);
        TotalRequestedDays = totalRequestedDays;

        return Result.Success();
    }
}
