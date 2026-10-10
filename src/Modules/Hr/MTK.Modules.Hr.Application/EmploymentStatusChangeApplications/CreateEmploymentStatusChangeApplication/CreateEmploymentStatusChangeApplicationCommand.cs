using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Domain.Employees;

namespace MTK.Modules.Hr.Application.EmploymentStatusChangeApplications.CreateEmploymentStatusChangeApplication;

/// <summary>
/// İş rejimi dəyişikliyi ərizəsi yaradır
/// </summary>
public sealed class CreateEmploymentStatusChangeApplicationCommand : ICommand<CreateEmploymentStatusChangeApplicationResponse>
{
    public Guid EmployeeId { get; set; }
    public EmploymentType NewEmploymentType { get; set; }
    public Guid OrderExecutionSupervisorId { get; set; }
}
