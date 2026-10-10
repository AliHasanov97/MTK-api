using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.NoticesOfChangeInWorkingConditions.DeleteNoticeOfChangeInWorkingConditions;

public sealed class DeleteNoticeOfChangeInWorkingConditionsCommand : ICommand
{
    public Guid Id { get; set; }
}