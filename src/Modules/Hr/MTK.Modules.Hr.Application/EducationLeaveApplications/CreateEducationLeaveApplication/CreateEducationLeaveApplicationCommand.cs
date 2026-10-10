using MTK.Common.Presentation.Responses;
using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.EducationLeaveApplications.CreateEducationLeaveApplication;

/// <summary>
/// Təhsil məzuniyyəti ərizəsi yaradır
/// </summary>
public sealed class CreateEducationLeaveApplicationCommand : ICommand<ResponseObjectWithName>
{
    public Guid EmployeeId { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public string? Reason { get; set; }
}
