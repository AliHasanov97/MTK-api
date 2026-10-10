using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.UnexcusedAbsences.DeleteUnexcusedAbsence;

public sealed class DeleteUnexcusedAbsenceCommand : ICommand
{
    public Guid Id { get; set; }
}
