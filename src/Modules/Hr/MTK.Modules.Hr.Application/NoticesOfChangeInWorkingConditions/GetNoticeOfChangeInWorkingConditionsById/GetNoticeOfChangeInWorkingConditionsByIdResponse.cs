using MTK.Common.Presentation.Responses;

namespace MTK.Modules.Hr.Application.NoticesOfChangeInWorkingConditions.GetNoticeOfChangeInWorkingConditionsById;

public sealed class GetNoticeOfChangeInWorkingConditionsByIdResponse
{
    public Guid Id { get; init; }
    public int Index { get; init; }
    public ResponseObjectWithName? Employee { get; init; }
    public DateTimeOffset StartDate { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public ResponseObjectWithName? CreatedBy { get; init; }
}