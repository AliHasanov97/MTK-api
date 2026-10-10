using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.UnexcusedAbsences.AddUnexcusedAbsence;

public sealed class AddUnexcusedAbsenceCommand : ICommand<AddUnexcusedAbsenceResponse>
{
    public Guid EmployeeId { get; set; }
    public DateTimeOffset SetDate { get; set; }
}
