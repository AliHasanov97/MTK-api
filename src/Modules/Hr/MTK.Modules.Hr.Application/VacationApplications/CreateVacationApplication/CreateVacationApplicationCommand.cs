using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.VacationApplications.CreateVacationApplication;

/// <summary>
/// Ödənişli məzuniyyət ərizəsi yaratmaq
/// EndDate VƏ YA RequestedDays göndərilməlidir (hər ikisi yox)
/// </summary>
public sealed record CreateVacationApplicationCommand(
    Guid EmployeeId,
    DateTimeOffset StartDate,
    DateTimeOffset? EndDate,         // Nullable: EndDate OR RequestedDays
    int? RequestedDays,              // Nullable: EndDate OR RequestedDays
    string? Notes
) : ICommand<CreateVacationApplicationResponse>;