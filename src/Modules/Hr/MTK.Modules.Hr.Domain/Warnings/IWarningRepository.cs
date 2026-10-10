using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.Warnings;

public interface IWarningRepository : IRepository<WarningOrder>
{
    Task<WarningOrder?> GetByIdForExportAsync(Guid id, CancellationToken cancellationToken = default);
}