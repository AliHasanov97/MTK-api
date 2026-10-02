using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.AuditLogs;

namespace MTK.Modules.Payments.Domain.Repositories;

public interface IAuditLogRepository : IRepository<AuditLog>
{
}
