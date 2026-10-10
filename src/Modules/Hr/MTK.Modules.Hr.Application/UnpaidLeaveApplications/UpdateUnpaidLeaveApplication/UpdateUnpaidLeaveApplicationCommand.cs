using System.Text.Json.Serialization;
using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.UnpaidLeaveApplications.UpdateUnpaidLeaveApplication;

/// <summary>
/// Ödənişsiz məzuniyyət ərizəsini yeniləyir (yalnız convert olmamışsa)
/// </summary>
public sealed class UpdateUnpaidLeaveApplicationCommand : ICommand
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public string? Notes { get; set; }
}