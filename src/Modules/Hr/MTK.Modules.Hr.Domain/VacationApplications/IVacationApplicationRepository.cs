using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.VacationApplications;

public interface IVacationApplicationRepository : IRepository<VacationApplication>
{
    /// <summary>
    /// ID ilə vacation application götür
    /// </summary>
    Task<VacationApplication?> GetByIdWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// İşçinin bütün ödənişli məzuniyyət müraciətlərini götür
    /// </summary>
    Task<List<VacationApplication>> GetByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);
}
