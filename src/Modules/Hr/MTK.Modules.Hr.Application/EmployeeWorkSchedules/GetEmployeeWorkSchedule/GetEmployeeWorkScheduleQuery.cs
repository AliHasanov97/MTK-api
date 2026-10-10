using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.EmployeeWorkSchedules.GetEmployeeWorkSchedule;

public sealed class GetEmployeeWorkScheduleQuery : IQuery<GetEmployeeWorkScheduleResponse>
{
    public Guid EmployeeId { get; set; }
}
