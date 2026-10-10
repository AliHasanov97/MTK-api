using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.VacationReturnApplications;

public interface IVacationReturnApplicationRepository : IRepository<VacationReturnApplication>
{
    /// <summary>
    /// ID ilə vacation return application götür (Employee və Company daxil)
    /// </summary>
    Task<VacationReturnApplication?> GetByIdWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// İşçinin bütün məzuniyyətdən geri qayıtma ərizələrini götür
    /// </summary>
    Task<List<VacationReturnApplication>> GetByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);
}