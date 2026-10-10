using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.ApplicationsForChangeOfPosition.AddApplicationForChangeOfPosition;

public sealed class AddApplicationForChangeOfPositionCommand : ICommand<AddApplicationForChangeOfPositionResponse>
{
    public Guid EmployeeId { get; set; }
    public Guid NewJobId { get; set; }
    public DateTimeOffset SetDate { get; set; }
}
