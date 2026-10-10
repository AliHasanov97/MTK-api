using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.EmployeeWorkSchedules;

public interface IEmployeeWorkScheduleRepository : IRepository<EmployeeWorkSchedule>
{
    Task<EmployeeWorkSchedule?> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default);
    Task<EmployeeWorkSchedule?> GetByEmployeeAndDateAsync(Guid employeeId, DateOnly effectiveFrom, CancellationToken cancellationToken = default);
    Task<List<EmployeeWorkSchedule>> GetAllByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default);
}
