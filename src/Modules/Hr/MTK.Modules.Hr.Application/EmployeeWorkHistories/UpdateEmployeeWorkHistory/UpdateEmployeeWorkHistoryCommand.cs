using MTK.Common.Application.Messaging;
using System.Text.Json.Serialization;

namespace MTK.Modules.Hr.Application.EmployeeWorkHistories.UpdateEmployeeWorkHistory;

public sealed class UpdateEmployeeWorkHistoryCommand : ICommand<UpdateEmployeeWorkHistoryResponse>
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public string? CompanyName { get; set; }
    public string? Position { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public string? Notes { get; set; }
}
