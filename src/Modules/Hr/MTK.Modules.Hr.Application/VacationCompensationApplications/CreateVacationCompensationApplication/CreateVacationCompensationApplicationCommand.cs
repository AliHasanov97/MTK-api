using MTK.Common.Presentation.Responses;
using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.VacationCompensationApplications.CreateVacationCompensationApplication;

/// <summary>
/// Məzuniyyət kompensasiyası ərizəsi yaradır
/// </summary>
public sealed class CreateVacationCompensationApplicationCommand : ICommand<ResponseObjectWithName>
{
    public Guid EmployeeId { get; set; }
    public Guid VacationBalanceId { get; set; }
    public int RequestedDays { get; set; }
    public string? Notes { get; set; }
}
