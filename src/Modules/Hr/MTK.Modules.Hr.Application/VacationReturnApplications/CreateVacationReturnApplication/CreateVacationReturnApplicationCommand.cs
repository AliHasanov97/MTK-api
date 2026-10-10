using MTK.Common.Application.Messaging;
using MTK.Common.Presentation.Responses;

namespace MTK.Modules.Hr.Application.VacationReturnApplications.CreateVacationReturnApplication;

/// <summary>
/// Məzuniyyətdən geri qayıtma ərizəsi yaratmaq
/// </summary>
public sealed record CreateVacationReturnApplicationCommand(
    Guid EmployeeId,
    DateTimeOffset ReturnDate,
    string? Notes
) : ICommand<ResponseObjectWithName>;