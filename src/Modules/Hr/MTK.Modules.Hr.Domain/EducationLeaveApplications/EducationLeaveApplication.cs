using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Helpers;
using MTK.Modules.Hr.Domain.Orders;
using MTK.Modules.Hr.Domain.EducationLeaveOrders;

namespace MTK.Modules.Hr.Domain.EducationLeaveApplications;

// Note: EducationLeaveApplicationErrors moved to Application layer
// Import from: MTK.Modules.Hr.Application.EducationLeaveApplications

/// <summary>
/// Təhsil məzuniyyəti müraciəti
/// </summary>
public sealed class EducationLeaveApplication : Application
{
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
    /// Bu ərizədən yaradılmış əmr (nullable - convert olunmamış ərizələrdə null ola bilər)
    /// </summary>
    public EducationLeaveOrder? EducationLeaveOrder { get; private set; }

    // Override base properties
    public override Employee? RelatedEmployee => Employee;
    public override Order? RelatedOrder => EducationLeaveOrder;

    private EducationLeaveApplication() { }

    /// <summary>
    /// Təhsil məzuniyyəti müraciəti yaradır
    /// </summary>
    public static Result<EducationLeaveApplication> Create(
        Guid employeeId,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        Guid createdById,
        string? reason = null)
    {
        // Validation: end date must be after start date
        if (endDate.Date < startDate.Date)
            return Result.Failure<EducationLeaveApplication>(
                new Error("EducationLeaveApplication.InvalidDates", "Bitmə tarixi başlama tarixindən əvvəl ola bilməz"));

        // Validation: start date cannot be in the past
        if (startDate.Date < DateTimeOffset.UtcNow.Date)
            return Result.Failure<EducationLeaveApplication>(
                new Error("EducationLeaveApplication.StartDateInPast", "Başlama tarixi keçmişdə ola bilməz"));

        var id = Guid.NewGuid();
        var application = new EducationLeaveApplication
        {
            Id = id,
            Type = ApplicationType.EducationLeave,
            Status = ApplicationStatus.PendingApproval,
            EmployeeId = employeeId,
            Reason = reason,
            StartDate = DateTimeHelper.ToUtcDateOnly(startDate),
            EndDate = DateTimeHelper.ToUtcDateOnly(endDate),
            CreatedById = createdById
        };

        return Result.Success(application);
    }

    /// <summary>
    /// Məlumatları yenilə
    /// </summary>
    public Result Update(DateTimeOffset startDate, DateTimeOffset endDate, string? reason = null)
    {
        if (endDate.Date < startDate.Date)
            return Result.Failure(
                new Error("EducationLeaveApplication.InvalidDates", "Bitmə tarixi başlama tarixindən əvvəl ola bilməz"));

        if (startDate.Date < DateTimeOffset.UtcNow.Date)
            return Result.Failure(
                new Error("EducationLeaveApplication.StartDateInPast", "Başlama tarixi keçmişdə ola bilməz"));

        Reason = reason;
        StartDate = DateTimeHelper.ToUtcDateOnly(startDate);
        EndDate = DateTimeHelper.ToUtcDateOnly(endDate);

        return Result.Success();
    }
}