using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.AuditLogs;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

internal sealed class AuditLogRepository : SearchableRepository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }
}
