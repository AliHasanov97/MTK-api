namespace MTK.Modules.Identity.Domain.AuditLogs;

public interface IAuditLogRepository
{
    Task<IReadOnlyList<AuditLog>> SearchAsync(AuditLogFilter filter, CancellationToken cancellationToken = default);
    void Add(AuditLog auditLog);
}
