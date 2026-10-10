using MTK.Common.Presentation.Responses;
using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.VacationCompensationApplications.CreateVacationCompensationApplication;

/// <summary>
/// Məzuniyyət kompensasiyası ərizəsi yaradır
/// </summary>
public sealed class CreateVacationCompensationApplicationCommand : ICommand<ResponseObjectWithName>
{
    public Guid EmployeeId { get; set; }
    public int RequestedDays { get; set; }

    /// <summary>Kompensasiya ediləcək iş ilinin başlanğıcı (ixtiyari)</summary>
    public DateOnly? WorkYearStart { get; set; }

    /// <summary>Kompensasiya ediləcək iş ilinin sonu (ixtiyari)</summary>
    public DateOnly? WorkYearEnd { get; set; }
    public string? Notes { get; set; }
}
