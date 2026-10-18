using MTK.Common.Domain.Abstractions;
using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.CompensationOrders;
using Microsoft.EntityFrameworkCore;

using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class CompensationOrderRepository
    : Repository<CompensationOrder>, ICompensationOrderRepository
{
    public CompensationOrderRepository(HrDbContext context) : base(context) { }

    public override async Task<CompensationOrder?> GetByIdDefaultAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        // VacationBalance mütləqdir: PDF-də və UI-da "iş ili" dövrü (WorkYearStart-WorkYearEnd)
        // yalnız bu naviqasiyadan oxunur — include olmasa hissə boş çap olunur.
        return await Context.Set<CompensationOrder>()
            .Include(o => o.Employee)
                .ThenInclude(e => e.Job)
            .Include(o => o.VacationBalance)
            .Include(o => o.CreatedBy)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public override async Task<List<CompensationOrder>> SearchAsync(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria,
        string? searchTerm,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFiltersAndSort(filters, sortCriteria);
        query = ApplyPages(query, page, pageSize);

        // Siyahıda yalnız işçi və yaradan göstərilir — "iş ili" (VacationBalance) detal
        // ekranında və PDF-də lazımdır (bax: GetByIdDefaultAsync).
        query = query
            .Include(o => o.Employee)
            .Include(o => o.CreatedBy);

        return await query.ToListAsync(cancellationToken);
    }
    
    public async Task<CompensationOrder?> GetByCompensationApplicationIdAsync(
        Guid compensationApplicationId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<CompensationOrder>()
            .FirstOrDefaultAsync(
                o => o.CompensationApplicationId == compensationApplicationId,
                cancellationToken);
    }

    public async Task<CompensationOrder?> GetByIdWithEmployeeAndCompanyAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<CompensationOrder>()
            .Include(o => o.Employee)
                .ThenInclude(e => e.Job)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }
}