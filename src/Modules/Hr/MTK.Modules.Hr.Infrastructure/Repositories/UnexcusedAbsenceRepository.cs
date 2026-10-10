using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.UnexcusedAbsences;
using Microsoft.EntityFrameworkCore;

using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class UnexcusedAbsenceRepository : SearchableRepository<UnexcusedAbsenceOrder>, IUnexcusedAbsenceRepository
{
    public UnexcusedAbsenceRepository(HrDbContext context) : base(context)
    {
    }

    public override async Task<UnexcusedAbsenceOrder?> GetByIdDefaultAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<UnexcusedAbsenceOrder>()
            .Include(a => a.Employee)
            .Include(a => a.Employee)
                .ThenInclude(e => e.Job)
            .Include(a => a.CreatedBy)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public override async Task<List<UnexcusedAbsenceOrder>> SearchAsync(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria,
        string? searchTerm,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<UnexcusedAbsenceOrder> query = ApplyFiltersAndSort(filters, sortCriteria)
            .Include(a => a.Employee)
            .Include(a => a.Employee)
                .ThenInclude(e => e.Job)
            .Include(a => a.CreatedBy);

        var searchTermQuery = GetSearchTerm(searchTerm);
        if (!string.IsNullOrWhiteSpace(searchTermQuery))
        {
            query = query.Where(p => p.SearchVector.Matches(EF.Functions.ToTsQuery(searchTermQuery)));
        }

        query = ApplyPages(query, page, pageSize);
        return await query.ToListAsync(cancellationToken);
    }
}
