using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.EmployeeEducationHistories;

public interface IEmployeeEducationHistoryRepository : IRepository<EmployeeEducationHistory>
{
    Task<List<EmployeeEducationHistory>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default);
    Task<EmployeeEducationHistory?> GetLatestByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default);
    Task<(List<EmployeeEducationHistory> Items, int TotalCount)> SearchByEmployeeIdAsync(Guid employeeId, string? searchTerm, int page, int pageSize, CancellationToken cancellationToken = default);
}
