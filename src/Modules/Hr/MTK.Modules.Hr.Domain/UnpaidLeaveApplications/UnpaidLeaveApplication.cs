using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Helpers;
using MTK.Modules.Hr.Domain.Orders;
using MTK.Modules.Hr.Domain.UnpaidLeaveOrders;

namespace MTK.Modules.Hr.Domain.UnpaidLeaveApplications;

// Note: UnpaidLeaveApplicationErrors moved to Application layer
// Import from: MTK.Modules.Hr.Application.UnpaidLeaveApplications

/// <summary>
/// Ödənişsiz məzuniyyət müraciəti
/// </summary>
public sealed class UnpaidLeaveApplication : Application
{
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
    /// Bu ərizədən yaradılmış əmr (nullable - convert olunmamış ərizələrdə null ola bilər)
    /// </summary>
    public UnpaidLeaveOrder? UnpaidLeaveOrder { get; private set; }

    // Override base properties
    public override Employee? RelatedEmployee => Employee;
    public override Order? RelatedOrder => UnpaidLeaveOrder;

    private UnpaidLeaveApplication() { }

    /// <summary>
    /// Ödənişsiz məzuniyyət müraciəti yaradır
    /// </summary>
    public static Result<UnpaidLeaveApplication> Create(
        Guid employeeId,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        Guid createdById,
        string? notes = null)
    {
        // Validation: end date must be after start date
        if (endDate.Date < startDate.Date)
            return Result.Failure<UnpaidLeaveApplication>(
                new Error("UnpaidLeaveApplication.InvalidDates", "Bitmə tarixi başlama tarixindən əvvəl ola bilməz"));

        // Validation: start date cannot be in the past
        if (startDate.Date < DateTimeOffset.UtcNow.Date)
            return Result.Failure<UnpaidLeaveApplication>(
                new Error("UnpaidLeaveApplication.StartDateInPast", "Başlama tarixi keçmişdə ola bilməz"));

        var id = Guid.NewGuid();
        var application = new UnpaidLeaveApplication
        {
            Id = id,
            Type = ApplicationType.UnpaidLeave,
            Status = ApplicationStatus.PendingApproval,
            EmployeeId = employeeId,
            StartDate = DateTimeHelper.ToUtcDateOnly(startDate),
            EndDate = DateTimeHelper.ToUtcDateOnly(endDate),
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
    public Result UpdateDates(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        if (endDate.Date < startDate.Date)
            return Result.Failure(
                new Error("UnpaidLeaveApplication.InvalidDates", "Bitmə tarixi başlama tarixindən əvvəl ola bilməz"));

        if (startDate.Date < DateTimeOffset.UtcNow.Date)
            return Result.Failure(
                new Error("UnpaidLeaveApplication.StartDateInPast", "Başlama tarixi keçmişdə ola bilməz"));

        StartDate = DateTimeHelper.ToUtcDateOnly(startDate);
        EndDate = DateTimeHelper.ToUtcDateOnly(endDate);

        return Result.Success();
    }
}
