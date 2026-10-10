using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Domain.Warnings;

namespace MTK.Modules.Hr.Application.Warnings.AddWarning;

public sealed class AddWarningCommand : ICommand<AddWarningResponse>
{
    public DisciplinaryType DisciplinaryType { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid OrderExecutionSupervisorId { get; set; }
    public DateTimeOffset SetDate { get; set; }
}
