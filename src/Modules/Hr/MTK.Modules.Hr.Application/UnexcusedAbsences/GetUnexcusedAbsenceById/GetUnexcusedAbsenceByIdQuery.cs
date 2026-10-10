using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.UnexcusedAbsences.GetUnexcusedAbsenceById;

public sealed class GetUnexcusedAbsenceByIdQuery : IQuery<GetUnexcusedAbsenceByIdResponse>
{
    public Guid Id { get; set; }
}
