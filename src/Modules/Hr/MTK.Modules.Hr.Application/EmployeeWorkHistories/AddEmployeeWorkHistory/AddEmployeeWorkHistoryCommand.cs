using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.EmployeeWorkHistories.AddEmployeeWorkHistory;

public sealed class AddEmployeeWorkHistoryCommand : ICommand<AddEmployeeWorkHistoryResponse>
{
    public Guid EmployeeId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public string? Notes { get; set; }
}
