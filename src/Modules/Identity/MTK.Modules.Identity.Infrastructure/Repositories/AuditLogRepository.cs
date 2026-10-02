using MTK.Common.Infrastructure.Database;
using MTK.Modules.Identity.Domain.AuditLogs;
using MTK.Modules.Identity.Infrastructure.Database;
using MTK.Modules.Identity.Domain.AuditLogs;

namespace MTK.Modules.Identity.Infrastructure.Repositories;

internal sealed class AuditLogRepository : SearchableRepository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(IdentityDbContext dbContext) : base(dbContext)
    {
    }
}