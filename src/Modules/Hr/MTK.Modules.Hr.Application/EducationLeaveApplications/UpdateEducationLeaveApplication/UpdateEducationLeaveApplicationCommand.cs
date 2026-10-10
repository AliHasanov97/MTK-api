using System.Text.Json.Serialization;
using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.EducationLeaveApplications.UpdateEducationLeaveApplication;

/// <summary>
/// Təhsil məzuniyyəti ərizəsini yeniləyir (yalnız convert olmamışsa)
/// </summary>
public sealed class UpdateEducationLeaveApplicationCommand : ICommand
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public string? Reason { get; set; }
}
