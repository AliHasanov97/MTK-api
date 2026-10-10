using System.Text.Json.Serialization;
using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Domain.Employees;

namespace MTK.Modules.Hr.Application.EmploymentStatusChangeApplications.UpdateEmploymentStatusChangeApplication;

/// <summary>
/// İş rejimi dəyişikliyi ərizəsini yeniləyir (yalnız əmrə çevrilməmişsə)
/// </summary>
public sealed class UpdateEmploymentStatusChangeApplicationCommand : ICommand
{
    [JsonIgnore]
    public Guid Id { get; set; }
    
    public EmploymentType? NewEmploymentType { get; set; }                                                                            

    public Guid OrderExecutionSupervisorId { get; set; }
}
