using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.EmploymentStatusChangeApplications;

public interface IEmploymentStatusChangeApplicationRepository : IRepository<EmploymentStatusChangeApplication>
{
    Task<EmploymentStatusChangeApplication?> GetByIdForExportAsync(Guid id, CancellationToken cancellationToken = default);
}