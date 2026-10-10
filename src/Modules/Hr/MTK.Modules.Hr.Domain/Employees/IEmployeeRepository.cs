using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.Employees;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<int> GetNextRegisterNumberAsync(CancellationToken cancellationToken = default);
    Task<Employee?> GetByIdWithWorkHistoriesAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Şirkətin işçilərini Department və Position ilə birlikdə qaytarır
    /// O ayda işləyən işçilər (StartWorkDate <= ayın sonu)
    /// </summary>
    Task<List<Employee>> GetForTimesheetAsync(
        DateOnly monthEnd,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Şirkətin aktiv işçilərini Department və Position ilə birlikdə qaytarır
    /// </summary>
    Task<List<Employee>> GetAllAsync(
        CancellationToken cancellationToken = default);
}
