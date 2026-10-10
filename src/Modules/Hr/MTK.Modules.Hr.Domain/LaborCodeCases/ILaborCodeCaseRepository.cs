using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.LaborCodeCases;

public interface ILaborCodeCaseRepository : IRepository<LaborCodeCase>
{
    Task<IReadOnlyList<LaborCodeCase>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LaborCodeCase>> GetRootCasesAsync(CancellationToken cancellationToken = default);
}
