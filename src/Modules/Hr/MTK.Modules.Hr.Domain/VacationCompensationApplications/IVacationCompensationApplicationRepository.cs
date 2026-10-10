using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.Applications;

namespace MTK.Modules.Hr.Domain.VacationCompensationApplications;

public interface IVacationCompensationApplicationRepository : IRepository<VacationCompensationApplication>
{
    /// <summary>
    /// ID ilə vacation compensation application götür (lines ilə birlikdə)
    /// </summary>
    Task<VacationCompensationApplication?> GetByIdWithLinesAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// İşçinin bütün kompensasiya müraciətlərini götür
    /// </summary>
    Task<List<VacationCompensationApplication>> GetByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);
}