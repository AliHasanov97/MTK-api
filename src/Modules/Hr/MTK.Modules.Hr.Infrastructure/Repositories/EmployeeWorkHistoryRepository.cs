using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.EmployeeWorkHistories;
using Microsoft.EntityFrameworkCore;

using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class EmployeeWorkHistoryRepository : Repository<EmployeeWorkHistory>, IEmployeeWorkHistoryRepository
{
    public EmployeeWorkHistoryRepository(HrDbContext context) : base(context) { }

    public async Task<List<EmployeeWorkHistory>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<EmployeeWorkHistory>()
            .Where(wh => wh.EmployeeId == employeeId)
            .OrderByDescending(wh => wh.StartDate)
            .ToListAsync(cancellationToken);
    }
}
