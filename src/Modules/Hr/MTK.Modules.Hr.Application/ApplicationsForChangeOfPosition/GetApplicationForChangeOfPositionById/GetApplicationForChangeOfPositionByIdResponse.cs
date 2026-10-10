using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Application.FileAttachments;

namespace MTK.Modules.Hr.Application.ApplicationsForChangeOfPosition.GetApplicationForChangeOfPositionById;

public sealed class GetApplicationForChangeOfPositionByIdResponse
{
    public Guid Id { get; init; }
    public int ApplicationNumber { get; init; }
    public string Status { get; init; } = null!;
    public ResponseObjectWithName? Employee { get; init; }
    public ResponseObjectWithName? CurrentJob { get; init; }
    public ResponseObjectWithName? NewJob { get; init; }
    public DateTimeOffset SetDate { get; init; }
    public ResponseObjectWithName? OrderForChangeOfPosition { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public ResponseObjectWithName? CreatedBy { get; init; }
}
