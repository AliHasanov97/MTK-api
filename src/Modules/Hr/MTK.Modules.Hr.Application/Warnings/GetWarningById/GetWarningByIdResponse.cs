using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Domain.Warnings;

namespace MTK.Modules.Hr.Application.Warnings.GetWarningById;

public sealed class GetWarningByIdResponse
{
    public Guid Id { get; init; }
    public int OrderNumber { get; init; }
    public DisciplinaryType DisciplinaryType { get; init; }
    public ResponseObjectWithName? Employee { get; init; }
    public ResponseObjectWithName? OrderExecutionSupervisor { get; init; }
    public DateTimeOffset SetDate { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public ResponseObjectWithName? CreatedBy { get; init; }
}
