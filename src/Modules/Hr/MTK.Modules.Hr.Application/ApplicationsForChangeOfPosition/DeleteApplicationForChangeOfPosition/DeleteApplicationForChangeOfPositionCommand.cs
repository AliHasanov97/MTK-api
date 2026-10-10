using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.ApplicationsForChangeOfPosition.DeleteApplicationForChangeOfPosition;

public sealed class DeleteApplicationForChangeOfPositionCommand : ICommand
{
    public Guid Id { get; set; }
}
