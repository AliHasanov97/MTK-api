using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.EmployeeWorkHistories;

public interface IEmployeeWorkHistoryRepository : IRepository<EmployeeWorkHistory>
{
    Task<List<EmployeeWorkHistory>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default);
}
