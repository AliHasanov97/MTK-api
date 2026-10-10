using MTK.Common.Presentation.Responses;

namespace MTK.Modules.Hr.Application.UnexcusedAbsences.GetUnexcusedAbsenceById;

public sealed class GetUnexcusedAbsenceByIdResponse
{
    public Guid Id { get; init; }
    public int OrderNumber { get; init; }
    public ResponseObjectWithName? Employee { get; init; }
    public DateTimeOffset SetDate { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public ResponseObjectWithName? CreatedBy { get; init; }
}
