namespace MTK.Modules.Hr.Application.VacationCompensationApplications.CreateVacationCompensationApplication;

public sealed class CreateVacationCompensationApplicationResponse
{
    public Guid Id { get; set; }
    public int ApplicationNumber { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int RequestedDays { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}