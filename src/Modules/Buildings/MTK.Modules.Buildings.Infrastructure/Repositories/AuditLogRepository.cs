using MTK.Common.Infrastructure.Database;
using MTK.Modules.Buildings.Domain.AuditLogs;
using MTK.Modules.Buildings.Domain.Repositories;
using MTK.Modules.Buildings.Infrastructure.Database;

namespace MTK.Modules.Buildings.Infrastructure.Repositories;

internal sealed class AuditLogRepository : SearchableRepository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(BuildingsDbContext dbContext) : base(dbContext)
    {
    }
}
