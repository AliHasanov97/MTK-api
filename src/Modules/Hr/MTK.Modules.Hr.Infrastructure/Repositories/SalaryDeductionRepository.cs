using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.SalaryDeductions;
using Microsoft.EntityFrameworkCore;

using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class SalaryDeductionRepository : SearchableRepository<SalaryDeduction>, ISalaryDeductionRepository
{
    public SalaryDeductionRepository(HrDbContext context) : base(context)
    {
    }

    public override async Task<SalaryDeduction?> GetByIdDefaultAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<SalaryDeduction>()
            .Include(sd => sd.Employee)
            .Include(sd => sd.CreatedBy)
            .Include(sd => sd.FileAttachments)
            .FirstOrDefaultAsync(sd => sd.Id == id, cancellationToken);
    }

    public async Task<SalaryDeduction?> GetByIdForExportAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<SalaryDeduction>()
            .Include(sd => sd.Employee)
            .Include(sd => sd.Employee)
                .ThenInclude(e => e.Job)
            .Include(sd => sd.CreatedBy)
            .FirstOrDefaultAsync(sd => sd.Id == id, cancellationToken);
    }

    public override async Task<List<SalaryDeduction>> SearchAsync(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria,
        string? searchTerm,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<SalaryDeduction> query = ApplyFiltersAndSort(filters, sortCriteria)
            .Include(sd => sd.Employee)
            .Include(sd => sd.CreatedBy);

        var searchTermQuery = GetSearchTerm(searchTerm);
        if (!string.IsNullOrWhiteSpace(searchTermQuery))
        {
            query = query.Where(p => p.SearchVector.Matches(EF.Functions.ToTsQuery(searchTermQuery)));
        }

        query = ApplyPages(query, page, pageSize);
        return await query.ToListAsync(cancellationToken);
    }
}