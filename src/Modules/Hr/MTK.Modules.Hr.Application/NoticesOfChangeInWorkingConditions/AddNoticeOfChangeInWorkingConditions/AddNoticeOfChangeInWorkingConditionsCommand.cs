using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.NoticesOfChangeInWorkingConditions.AddNoticeOfChangeInWorkingConditions;

public sealed class AddNoticeOfChangeInWorkingConditionsCommand : ICommand<AddNoticeOfChangeInWorkingConditionsResponse>
{
    public Guid EmployeeId { get; set; }
    public DateTimeOffset StartDate { get; set; }
}