using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Users;

namespace MTK.Modules.Hr.Domain.EmployeeWorkHistories;

public sealed class EmployeeWorkHistory : Entity
{
    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; }
    public string CompanyName { get; private set; } = string.Empty;
    public string Position { get; private set; } = string.Empty;
    public DateTimeOffset StartDate { get; private set; }
    public DateTimeOffset? EndDate { get; private set; }
    public string? Notes { get; private set; }
    public Guid CreatedById { get; private set; }
    public User CreatedBy { get; private set; } = null!;

    private EmployeeWorkHistory() { }

    public static EmployeeWorkHistory Create(
        Guid employeeId,
        string companyName,
        string position,
        DateTimeOffset startDate,
        DateTimeOffset? endDate,
        string? notes,
        Guid createdById)
    {
        var workHistory = new EmployeeWorkHistory
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            CompanyName = companyName,
            Position = position,
            StartDate = startDate,
            EndDate = endDate,
            Notes = notes,
            CreatedById = createdById
        };

        workHistory.RaiseDomainEvent(new EmployeeWorkHistoryChangedDomainEvent { EmployeeId = employeeId });

        return workHistory;
    }

    public void Update(
        string? companyName,
        string? position,
        DateTimeOffset? startDate,
        DateTimeOffset? endDate,
        string? notes)
    {
        if (companyName is not null) CompanyName = companyName;
        if (position is not null) Position = position;
        if (startDate.HasValue) StartDate = startDate.Value;
        if (endDate.HasValue) EndDate = endDate.Value;
        if (notes is not null) Notes = string.IsNullOrWhiteSpace(notes) ? null : notes;

        RaiseDomainEvent(new EmployeeWorkHistoryChangedDomainEvent { EmployeeId = EmployeeId });
    }

    /// <summary>
    /// WorkHistory silinməzdən əvvəl çağırılır
    /// Silinmə domain event-i atır
    /// </summary>
    public void Delete()
    {
        RaiseDomainEvent(new EmployeeWorkHistoryChangedDomainEvent { EmployeeId = EmployeeId });
    }

    public Employees.WorkExperienceBreakdown CalculateDuration()
    {
        return Employees.WorkExperienceBreakdown.FromDateRange(StartDate, EndDate);
    }
}
