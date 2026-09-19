using Microsoft.EntityFrameworkCore;
using MTK.Modules.Identity.Domain.AuditLogs;
using MTK.Modules.Identity.Infrastructure.Database;

namespace MTK.Modules.Identity.Infrastructure.Repositories;

internal sealed class AuditLogRepository : IAuditLogRepository
{
    private readonly IdentityDbContext _context;

    public AuditLogRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<AuditLog>> SearchAsync(AuditLogFilter filter, CancellationToken cancellationToken = default)
    {
        var query = _context.AuditLogs.AsQueryable();

        if (!string.IsNullOrEmpty(filter.EntityType))
        {
            query = query.Where(al => al.EntityType == filter.EntityType);
        }

        if (filter.EntityId.HasValue)
        {
            query = query.Where(al => al.EntityId == filter.EntityId.Value);
        }

        if (!string.IsNullOrEmpty(filter.Action))
        {
            query = query.Where(al => al.Action == filter.Action);
        }

        if (filter.UserId.HasValue)
        {
            query = query.Where(al => al.UserId == filter.UserId.Value);
        }

        if (filter.DateFrom.HasValue)
        {
            query = query.Where(al => al.Timestamp >= filter.DateFrom.Value);
        }

        if (filter.DateTo.HasValue)
        {
            query = query.Where(al => al.Timestamp <= filter.DateTo.Value);
        }

        return await query
            .OrderByDescending(al => al.Timestamp)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);
    }

    public void Add(AuditLog auditLog)
    {
        _context.AuditLogs.Add(auditLog);
    }
}
