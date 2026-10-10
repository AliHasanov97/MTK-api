using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.SalaryDeductions;

public interface ISalaryDeductionRepository : IRepository<SalaryDeduction>
{
    Task<SalaryDeduction?> GetByIdForExportAsync(Guid id, CancellationToken cancellationToken = default);
}