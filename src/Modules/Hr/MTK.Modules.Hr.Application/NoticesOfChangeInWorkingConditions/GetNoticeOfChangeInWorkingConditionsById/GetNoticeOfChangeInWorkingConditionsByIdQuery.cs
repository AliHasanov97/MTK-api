using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.NoticesOfChangeInWorkingConditions.GetNoticeOfChangeInWorkingConditionsById;

public sealed class GetNoticeOfChangeInWorkingConditionsByIdQuery : IQuery<GetNoticeOfChangeInWorkingConditionsByIdResponse>
{
    public Guid Id { get; set; }
}