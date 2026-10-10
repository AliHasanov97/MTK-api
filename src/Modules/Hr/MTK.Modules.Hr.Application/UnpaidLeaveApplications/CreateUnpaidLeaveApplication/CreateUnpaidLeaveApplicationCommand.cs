using MTK.Common.Presentation.Responses;
using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.UnpaidLeaveApplications.CreateUnpaidLeaveApplication;

/// <summary>
/// Ödənişsiz məzuniyyət ərizəsi yaradır
/// </summary>
public sealed class CreateUnpaidLeaveApplicationCommand : ICommand<ResponseObjectWithName>
{
    public Guid EmployeeId { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public string? Notes { get; set; }
}
